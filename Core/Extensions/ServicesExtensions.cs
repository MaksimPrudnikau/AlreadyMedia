using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfigurationManager configuration)
    {
        return services.AddStackExchangeRedisCache(options =>
        {
            var connection = configuration.GetConnectionString("Redis");
            options.Configuration = connection;
        });
    }
}