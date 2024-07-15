using Honeys_Kitchen_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Honeys_Kitchen_backend.Migrations;

public class HoneysKitchenDbContext : DbContext
{
    public DbSet<User> User {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.PhoneNumber);
            entity.Property(e => e.EmailAddress);
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Address);
        });
    }
}