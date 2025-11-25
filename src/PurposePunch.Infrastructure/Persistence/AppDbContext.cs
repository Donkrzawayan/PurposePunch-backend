using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurposePunch.Domain.Entities;
using PurposePunch.Infrastructure.Identity;

namespace PurposePunch.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Decision> Decisions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Decision>(entity =>
        {
            entity.Property(d => d.Title).IsRequired().HasMaxLength(60);

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .IsRequired();
        });
    }
}
