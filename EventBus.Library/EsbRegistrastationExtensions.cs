using EventBus.Library;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class EsbRegistrastationExtensions
{
    // TODO: check if name should be RegisterMessageProcessor or AddMessageProcessor
    public static IServiceCollection RegisterMessageProcessor<T>(this IServiceCollection services)
        where T : class, IBroker
    {
        services.AddHostedService<EventRegistractions>();
        services.TryAddTransient<IBroker, T>();

        return services;
    }
}
