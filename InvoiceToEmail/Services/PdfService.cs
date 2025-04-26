using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace InvoiceToEmail.Services
{
    public class PdfService
    {
        public byte[] CreateInvoicePdf(string customerName, int invoiceNumber, decimal price, DateTime date)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"فاتورة رقم: {invoiceNumber}").Bold().FontSize(24).AlignCenter();
                        column.Item().Text($"العميل: {customerName}");
                        column.Item().Text($"السعر: {price} ريال");
                        column.Item().Text($"التاريخ: {date.ToString("dd/MM/yyyy")}");
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        public void SendInvoiceEmail(string toEmail, byte[] pdfBytes, string fileName)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("لايميل الي بيرسل الفواتير", "باسورد الايميل"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("الايميل الي بيرسل الفواتير"),
                Subject = "فاتورة جديدة",
                Body = "مرفق مع الرسالة ملف الفاتورة بصيغة PDF.",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            mailMessage.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), fileName));

            smtpClient.Send(mailMessage);
        }
    }
}