using Microsoft.EntityFrameworkCore;
using Back.Models;

namespace Back.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<InvoicePayment> InvoicePayments { get; set; }
    public DbSet<CreditNote> CreditNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Payment)
            .WithOne(p => p.Invoice)
            .HasForeignKey<InvoicePayment>(p => p.InvoiceNumber);

        modelBuilder.Entity<CreditNote>()
            .HasKey(cn => cn.CreditNoteNumber); 

        modelBuilder.Entity<CreditNote>()
            .HasOne(cn => cn.Invoice)
            .WithMany(i => i.CreditNotes)
            .HasForeignKey(cn => cn.InvoiceNumber);

        
        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.InvoiceDetails)
            .WithOne(d => d.Invoice)
            .HasForeignKey(d => d.InvoiceNumber);

       
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany()  
            .HasForeignKey(i => i.CustomerRun)  
            .OnDelete(DeleteBehavior.Cascade);  

            
        // Configurar valores por defecto
        modelBuilder.Entity<Customer>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        modelBuilder.Entity<Invoice>()
            .Property(i => i.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

}