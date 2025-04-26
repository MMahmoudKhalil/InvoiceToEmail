using System.ComponentModel.DataAnnotations;

namespace InvoiceToEmail.Data.Model
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int VatNumber { get; set; }
        [Required]
        public int Phone { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }



        public virtual ICollection<Invoice> invoices { get; set; } = new List<Invoice>();
        
    }
}
