using InvoiceToEmail.Data;
using InvoiceToEmail.Data.Model;
using InvoiceToEmail.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InvoiceToEmail.Controllers
{
    public class HomePageController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly PdfService _pdfService;
        public HomePageController(ApplicationDbContext dbContext, PdfService pdfService)
        {
            this.dbContext = dbContext;
            this._pdfService = pdfService;
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Customer()
        {
            var customer = dbContext.customers.ToList();

            return View(customer);
        }
        public IActionResult Invoice()
        {
            var invoice = dbContext.invoices.ToList();
            return View(invoice);
        }
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customers)
        {
            if (ModelState.IsValid)
            {
                dbContext.customers.Add(new Customer
                {
                    Name      = customers.Name,
                    VatNumber = customers.VatNumber,
                    Phone     = customers.Phone,
                    Email     = customers.Email
                });
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "✔ Data added successfully!";
                return RedirectToAction("Customer");
            }

            return View(customers);
        }

        public IActionResult EditCustomer(int CuId)
        {
            var customers = dbContext.customers.FirstOrDefault(e => e.Id == CuId);
            if (customers == null)
            {
                return View("NotFound");
            }
            return View(customers);
        }

        [HttpPost]
        public IActionResult EditCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                dbContext.customers.Update(customer);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "✔ Data updated successfully!";
                return RedirectToAction("Customer");
            }
            return View(customer);
        }

        public IActionResult Delete(int CuId)
        {
            var customer = dbContext.customers.FirstOrDefault(e => e.Id == CuId);
            if (customer != null)
            {
                dbContext.customers.Remove(customer);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "✔ Customer deleted successfully!";
                return RedirectToAction("Customer");
            }
            return View("NotFund");

        }
               
        public IActionResult CreateInvoice()
        {
            var customers = dbContext.customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text  = c.Name
            }).ToList();
            ViewBag.Customers = customers;
            return View();
        }
        [HttpPost]
        public IActionResult CreateInvoice(Invoice invoice)
        {
            ModelState.Remove("Customer");

            if (ModelState.IsValid)
            {
                invoice.CreatedAt = DateTime.Now;
                invoice.UpdatedAt = DateTime.Now;

                int lastInvNumber = dbContext.invoices
                    .OrderByDescending(i => i.InvNumber)
                    .Select(i => i.InvNumber)
                    .FirstOrDefault();
                invoice.InvNumber = lastInvNumber + 1;

                dbContext.invoices.Add(invoice);
                dbContext.SaveChanges();

                
                var customer = dbContext.customers.FirstOrDefault(c => c.Id == invoice.CuId);
                byte[] pdfBytes = _pdfService.CreateInvoicePdf(
                    customer.Name,
                    invoice.InvNumber,
                    (decimal)invoice.Price,
                    invoice.CreatedAt
                );

                
                _pdfService.SendInvoiceEmail(customer.Email, pdfBytes, $"فاتورة_{invoice.InvNumber}.pdf");

                TempData["SuccessMessage"] = "✔ Data added and Email sent successfully!";
                return RedirectToAction("Invoice");
            }

            var customers = dbContext.customers.ToList();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            return View(invoice);
        }

    }
}

