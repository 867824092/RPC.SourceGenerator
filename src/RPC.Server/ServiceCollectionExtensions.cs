using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RPC.Abstracts.Interface;
using RPC.Server.Core;
using RPC.Server.Interface;
using RPC.Server.Internal;

namespace RPC.Server; 

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddRPCServer(this IServiceCollection services) =>
        AddRPCServer(services,null);
    public static IServiceCollection AddRPCServer(this IServiceCollection services,Action<IApplicationBuilder> builderAction) {
        services.AddSingleton<ISerializer, DefaultSerialzer>();
        services.AddSingleton<IApplicationBuilder>(provider => {
            var builder = new ApplicationBuilder(provider);
            builderAction?.Invoke(builder);
            builder.Use<EndpointResolveMiddleware>();
            return builder;
        });
        services.AddSingleton<TcpConnectionHandler>(provider => {
            var handler = new TcpConnectionHandler(provider.GetRequiredService<ILoggerFactory>())
            { RequestDelegate = provider.GetRequiredService<IApplicationBuilder>().Build() };
            return handler;
        });
        return services;
    }
}