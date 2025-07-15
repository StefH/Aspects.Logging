using Aspects.Logging.DependencyInjection;
using Aspects.Logging.Options;
using Aspects.Logging.Utils;
using Microsoft.Extensions.Configuration;
using Stef.Validation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspectLogging(this IServiceCollection services)
    {
        Guard.NotNull(services);

        return services.RegisterAspectLogging(new AspectLoggingOptions());
    }

    public static IServiceCollection RegisterAspectLogging(this IServiceCollection services, IConfigurationSection section)
    {
        Guard.NotNull(services);
        Guard.NotNull(section);

        var options = new AspectLoggingOptions();
        section.Bind(options);

        return services.RegisterAspectLogging(options);
    }

    public static IServiceCollection RegisterAspectLogging(this IServiceCollection services, Action<AspectLoggingOptions> configureAction)
    {
        Guard.NotNull(services);
        Guard.NotNull(configureAction);

        var options = new AspectLoggingOptions();
        configureAction(options);

        return services.RegisterAspectLogging(options);
    }

    public static IServiceCollection RegisterAspectLogging(this IServiceCollection services, AspectLoggingOptions options)
    {
        Guard.NotNull(services);
        Guard.NotNull(options);

        services.AddOptionsWithDataAnnotationValidation(options);

        services.InitializeAspectServiceLocator();

        return services.AddSingleton<IGuidProvider, GuidProvider>();
    }
}