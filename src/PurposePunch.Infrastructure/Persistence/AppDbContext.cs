using Microsoft.EntityFrameworkCore;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Decision> Decisions { get; set; }
}
