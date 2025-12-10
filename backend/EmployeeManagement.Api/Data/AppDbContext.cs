using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePhone> EmployeePhones => Set<EmployeePhone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DocNumber).IsUnique();

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DocNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity.HasMany(e => e.Phones)
                  .WithOne(p => p.Employee!)
                  .HasForeignKey(p => p.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.Subordinates)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeePhone>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(30);
        });
    }
}
