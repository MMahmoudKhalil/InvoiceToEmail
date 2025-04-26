using InvoiceToEmail.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace InvoiceToEmail.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Invoice> invoices { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ToEmail;Integrated Security=True;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Invoice>()
                    .HasOne(i => i.customer)
                    .WithMany(c => c.invoices)
                    .HasForeignKey(i => i.CuId);
            base.OnModelCreating(modelBuilder);

        }
    }
}
