using EventBus.WebApi.ESB;

namespace EventBus.WebApi;

public static class EsbRegistrastationExtensions
{
    // TODO: check if name should be RegisterMessageProcessor or AddMessageProcessor
    public static IServiceCollection RegisterMessageProcessor<T>(this IServiceCollection services) where T: IBroker
    {
        return services.AddTransient(typeof(IBroker), typeof(T));
    }
}
