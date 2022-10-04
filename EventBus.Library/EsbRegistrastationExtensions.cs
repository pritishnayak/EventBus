using EventBus.Library;

namespace Microsoft.Extensions.DependencyInjection;

public static class EsbRegistrastationExtensions
{
    // TODO: check if name should be RegisterMessageProcessor or AddMessageProcessor
    public static IServiceCollection RegisterMessageProcessor<T>(this IServiceCollection services)
        where T : class, IBroker
    {
        return services.AddTransient<IBroker, T>();
    }
}
