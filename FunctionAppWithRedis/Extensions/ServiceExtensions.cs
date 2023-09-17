using FunctionAppWithRedis.Interfaces;
using FunctionAppWithRedis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionAppWithRedis.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddMyService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPeachService, PeachService>();
        services.AddDistributedRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "RJ-";
        });

        return services;
    }
}
