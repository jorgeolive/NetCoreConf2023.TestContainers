using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace NetCoreConf2023.BlogApp.IntegrationTests;

public class RedisWaitStrategy : IWaitUntil
{
    public async Task<bool> UntilAsync(IContainer container)
    {
        var response = await container.ExecAsync(new List<string> { "redis-cli", "ping" });
        return response.Stdout.Contains("PONG");
    }
}
public class Infrastructure : IAsyncLifetime
{
    private IFutureDockerImage _nginxImage;
    private INetwork _dockerNetwork;
    public RedisContainer Redis;
    public PostgreSqlContainer PostgreSql;

    public IContainer NginxContainer { get; }

    public Infrastructure()
    {
        _dockerNetwork = new NetworkBuilder().Build(); //Network not neccesary, only for demonstration purposes

        Redis = new RedisBuilder().
            WithImage("redis:latest"). // RedisTestContainer defaults to v7
            WithNetwork(_dockerNetwork).
            WithWaitStrategy(
                Wait.ForUnixContainer().AddCustomWaitStrategy(new RedisWaitStrategy())). // Not neccesary, only for demonstration purposes
            Build();

        PostgreSql = new PostgreSqlBuilder().
            WithNetwork(_dockerNetwork).     // No need to provide WaitStrategy, see PostgresqlContainer internals
            Build();

        _nginxImage = new ImageFromDockerfileBuilder().
            WithDockerfileDirectory(Directory.GetCurrentDirectory()).
            WithDockerfile("Dockerfile").
            Build();

        NginxContainer = new ContainerBuilder().WithImage(_nginxImage).WithPortBinding(80, true).Build();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _nginxImage.CreateAsync();

        await Redis.StartAsync();
        await PostgreSql.StartAsync();
        await NginxContainer.StartAsync();
    }

    public async Task Reset()
    {
        await Redis.ExecAsync(new List<string> { "redis-cli", "FLUSHALL" });
        await ResetPostgresql();
    }

    private async Task ResetPostgresql()
    {
        using (var connection = new NpgsqlConnection(PostgreSql.GetConnectionString()))
        {
            await connection.OpenAsync();

            // Poorman's reset: Execute SQL commands to drop and recreate the database
            var dropDatabaseCommand = new NpgsqlCommand(@"DELETE FROM  public.""Blogs"";", connection);

            await dropDatabaseCommand.ExecuteNonQueryAsync();
        }
    }
}