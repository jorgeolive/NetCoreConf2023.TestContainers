﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetCoreConf2023.BlogApp.EntityFramework;

#nullable disable

namespace NetCoreConf2023.BlogApp.Migrations;

[DbContext(typeof(BlogAppDbContext))]
partial class BlogAppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "7.0.5")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("NetCoreConf2023.MyApplication.Models.Blog", b =>
            {
                b.Property<int>("BlogId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BlogId"));

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Url")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int?>("UserId")
                    .HasColumnType("integer");

                b.HasKey("BlogId");

                b.ToTable("Blogs");
            });
#pragma warning restore 612, 618
    }
}
