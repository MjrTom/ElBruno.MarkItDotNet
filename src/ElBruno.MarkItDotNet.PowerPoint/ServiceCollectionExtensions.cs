using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.PowerPoint;

/// <summary>
/// Extension methods for registering MarkItDotNet PowerPoint services with the DI container.
/// </summary>
public static class PowerPointServiceCollectionExtensions
{
    /// <summary>
    /// Adds the PowerPoint converter plugin to the MarkItDotNet converter registry.
    /// Call this after <c>AddMarkItDotNet()</c>.
    /// </summary>
    public static IServiceCollection AddMarkItDotNetPowerPoint(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ConverterRegistry));
        if (descriptor?.ImplementationInstance is ConverterRegistry registry)
        {
            registry.RegisterPlugin(new PowerPointPlugin());
        }

        return services;
    }
}
