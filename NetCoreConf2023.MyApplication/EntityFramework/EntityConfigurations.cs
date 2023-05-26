using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreConf2023.MyApplication.Models;

namespace NetCoreConf2023.BlogApp.EntityFramework;

public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder
            .Property(b => b.Url)
            .IsRequired();

        builder.HasKey(b => b.BlogId);
    }
}

