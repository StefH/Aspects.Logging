using Stef.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Aspects.Logging.DependencyInjection;

internal static class AspectServiceLocator
{
    private static Lazy<IServiceProvider>? _serviceProvider;

    public static void InitializeAspectServiceLocator(this IServiceProvider serviceProvider)
    {
        _serviceProvider = new(() => Guard.NotNull(serviceProvider));
    }

    public static void InitializeAspectServiceLocator(this IServiceCollection services)
    {
        _serviceProvider = new(Guard.NotNull(services).BuildServiceProvider);
    }

    public static Lazy<T?> GetService<T>() where T : notnull
    {
        return new(() => _serviceProvider != null ? _serviceProvider.Value.GetService<T>() : default);
    }
}