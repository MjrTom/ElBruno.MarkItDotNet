using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ElBruno.MarkItDotNet.AI;

/// <summary>
/// Extension methods for registering AI converters with dependency injection.
/// </summary>
public static class AiServiceCollectionExtensions
{
    /// <summary>
    /// Registers the AI converter plugin with the service collection.
    /// Requires an <see cref="IChatClient"/> to be registered in the container.
    /// </summary>
    public static IServiceCollection AddMarkItDotNetAI(
        this IServiceCollection services,
        Action<AiOptions>? configure = null)
    {
        var options = new AiOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IConverterPlugin>(sp =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return new AiConverterPlugin(chatClient, options);
        });

        return services;
    }
}
