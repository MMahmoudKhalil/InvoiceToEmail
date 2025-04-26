namespace InvoiceToEmail.Data.Model
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int InvNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public int CuId { get; set; }
        public Customer customer { get; set; }
    }
}
