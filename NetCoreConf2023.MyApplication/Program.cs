
using Microsoft.EntityFrameworkCore;
using NetCoreConf2023.BlogApp.Api;
using NetCoreConf2023.BlogApp.EntityFramework;
using NetCoreConf2023.MyApplication.Models;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IUsersService, UsersService>();

        builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse(builder.Configuration.GetSection("Redis:ConnectionString").Value!);
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.AddDbContext<BlogAppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("blogApp")));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapGet("/blogs/{blogId}", async (BlogAppDbContext dbContext, IConnectionMultiplexer redisFac, int blogId) =>
        {
            var redis = redisFac.GetDatabase();

            var blogFromCache = await redis.StringGetAsync(blogId.ToString());

            if (!blogFromCache.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Blog>(blogFromCache.ToString());
            }

            var blogFromDb = await dbContext.Blogs.SingleOrDefaultAsync(x => x.BlogId == blogId);

            if (blogFromDb != null)
            {
                await redis.StringSetAsync(blogId.ToString(), JsonSerializer.Serialize(blogFromDb));
            }

            return blogFromDb;
        })
        .WithName("GetBlogByName")
        .WithOpenApi();

        app.MapPost("/blogs", async (BlogAppDbContext dbContext, Blog blog) =>
        {
            await dbContext.Blogs.AddAsync(blog);
            await dbContext.SaveChangesAsync();

            return blog;
        })
        .WithName("CreateBlog")
        .WithOpenApi();

        app.MapGet("/blogs/{blogId}/user-details", async (int blogId, BlogAppDbContext dbContext, IUsersService userService) =>
        {
            var userId = await dbContext.Blogs.Where(x => x.BlogId == blogId).Select(x => x.UserId).SingleOrDefaultAsync();

            if (userId == null)
                return Results.NotFound();

            var details = await userService.GetDetailsFor(userId.Value);

            return Results.Ok(details);
        })
        .WithName("GetBlogUserDetails")
        .WithOpenApi();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();
        await dbContext.Database.MigrateAsync();

        app.Run();

public partial class Program { }