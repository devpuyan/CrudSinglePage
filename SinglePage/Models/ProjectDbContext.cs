using Microsoft.EntityFrameworkCore;
using SinglePage.Models.DomainModels.PersonAggregate;

namespace SinglePage.Models;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Person> Person { get; set; }
}