using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.Excel;

/// <summary>
/// Extension methods for registering MarkItDotNet Excel services with the DI container.
/// </summary>
public static class ExcelServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Excel converter plugin to the MarkItDotNet converter registry.
    /// Call this after <c>AddMarkItDotNet()</c>.
    /// </summary>
    public static IServiceCollection AddMarkItDotNetExcel(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ConverterRegistry));
        if (descriptor?.ImplementationInstance is ConverterRegistry registry)
        {
            registry.RegisterPlugin(new ExcelPlugin());
        }

        return services;
    }
}
