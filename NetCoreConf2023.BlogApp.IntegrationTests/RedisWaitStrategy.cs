using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace NetCoreConf2023.BlogApp.IntegrationTests;

public class RedisWaitStrategy : IWaitUntil
{
    public async Task<bool> UntilAsync(IContainer container)
    {
        var response = await container.ExecAsync(new List<string> { "redis-cli", "ping" });
        return response.Stdout.Contains("PONG");
    }
}
