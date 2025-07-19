using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlowingLinks.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructuralServices(this IServiceCollection services)
    {
        // Get all types in the current assembly that are in the FlowingLinks.Core.Services namespace
        var serviceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.Namespace == "FlowingLinks.Core.Services" && 
                          !type.IsAbstract && 
                          !type.IsInterface && 
                          type.IsClass);

        // Register each service type as scoped
        foreach (var serviceType in serviceTypes)
            services.AddScoped(serviceType);

        return services;
    }
}