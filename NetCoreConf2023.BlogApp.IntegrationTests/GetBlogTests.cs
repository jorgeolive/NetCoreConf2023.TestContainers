using Microsoft.Extensions.DependencyInjection;
using NetCoreConf2023.BlogApp.EntityFramework;
using NetCoreConf2023.MyApplication.Models;
using System.Net.Http.Json;

namespace NetCoreConf2023.BlogApp.IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTests), DisableParallelization = true)]
    public class IntegrationTests : ICollectionFixture<Infrastructure>, ICollectionFixture<WebIntegrationTestsFactory<Program>> { }

    [Collection(nameof(IntegrationTests))]
    public class BlogAppTests : IAsyncLifetime
    {
        private readonly WebIntegrationTestsFactory<Program> testServerFactory;
        private readonly Infrastructure infrastructure;

        public BlogAppTests(Infrastructure infrastructure, WebIntegrationTestsFactory<Program> testServerFactory)
        {
            this.testServerFactory = testServerFactory;
            this.infrastructure = infrastructure;

            Environment.SetEnvironmentVariable("Redis__ConnectionString", infrastructure.Redis.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__blogApp", infrastructure.PostgreSql.GetConnectionString());
        }

        public async Task DisposeAsync()
        {
            await infrastructure.Reset();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CanGetABlog()
        {

            using var scope = testServerFactory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();


            dbContext.Blogs.Add(new Blog() { BlogId = 1, Name = "aBlog", Url = "myUrl", UserId = 1 });
            await dbContext.SaveChangesAsync();

            var httpClient = this.testServerFactory.CreateClient();
            var blog = await httpClient.GetFromJsonAsync<Blog>("/blogs/1");

            Assert.Equivalent(new Blog() { BlogId = 1, Name = "aBlog", Url = "myUrl", UserId = 1 }, blog);
        }

        [Fact]
        public async Task WhenGettingABlog_IfItsNotInRedis_ItIsStoredInCache()
        {
            using var scope = testServerFactory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();

            var blogToQuery = new Blog() { BlogId = 1, Name = "aBlog", Url = "myUrl", UserId = 1 };

            dbContext.Blogs.Add(blogToQuery);
            await dbContext.SaveChangesAsync();

            var httpClient = this.testServerFactory.CreateClient();
            var _ = await httpClient.GetFromJsonAsync<Blog>("/blogs/1");

            var blogFromCache = await this.infrastructure.Redis.ExecAsync(new List<string> { "redis-cli", "GET","1" });

            Assert.Equivalent(System.Text.Json.JsonSerializer.Deserialize<Blog>(blogFromCache.Stdout), blogToQuery);
        }
    }
}