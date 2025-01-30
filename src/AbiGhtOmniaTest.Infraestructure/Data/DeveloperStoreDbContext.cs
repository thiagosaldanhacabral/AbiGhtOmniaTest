using AbiGhtOmniaTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbiGhtOmniaTest.Infraestructure.Data;

public class DeveloperStoreDbContext(DbContextOptions<DeveloperStoreDbContext> options) : DbContext(options)
{
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>().HasKey(s => s.SaleId);
        modelBuilder.Entity<SaleItem>().HasKey(si => si.SaleItemId);

        modelBuilder.Entity<Sale>()
            .HasMany(s => s.Items)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId);

        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().OwnsOne(u => u.Name);
        modelBuilder.Entity<User>().OwnsOne(u => u.Address).OwnsOne(a => a.Geolocation);
    }
}
