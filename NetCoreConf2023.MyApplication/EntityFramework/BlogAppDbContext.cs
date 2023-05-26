using Microsoft.EntityFrameworkCore;
using NetCoreConf2023.MyApplication.Models;
using System.Reflection;

namespace NetCoreConf2023.BlogApp.EntityFramework;

public class BlogAppDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public BlogAppDbContext(DbContextOptions<BlogAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
