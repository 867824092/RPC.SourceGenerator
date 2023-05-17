using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RPC.Abstracts.Interface;
using RPC.Server.Core;
using RPC.Server.Interface;

namespace RPC.Server.Internal; 

internal sealed class ApplicationBuilder : IApplicationBuilder {
    private readonly List<Func<RequestDelegate, RequestDelegate>> _components = new();
    public IServiceProvider ApplicationServices { get; set; }

    public ApplicationBuilder(IServiceProvider serviceProvider)
    {
        ApplicationServices = serviceProvider;
    }
    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        _components.Add(middleware);
        return this;
    }
    public IApplicationBuilder Use(RequestDelegate middleware) {
        if(middleware == null) throw new ArgumentNullException(nameof(middleware));
        RequestDelegate next = context => middleware(context);
        return Use(next);
    }
    
    public IApplicationBuilder Use<TMiddleware>()
        where TMiddleware : IMiddleware
    {
        var middleware = ActivatorUtilities.GetServiceOrCreateInstance<TMiddleware>(ApplicationServices);
        return Use(middleware.Invoke);
    }
    
    public IApplicationBuilder Use<TMiddleware>(TMiddleware middleware)
        where TMiddleware : IMiddleware
    {
        return Use(middleware.Invoke);
    }
    
    public IApplicationBuilder Use(Func<RequestDelegate, RPCRequestContext, Task> middleware)
    {
        return Use(next => context => middleware(next, context));
    }

    public RequestDelegate Build() {
        var logger = ApplicationServices.GetService<ILogger<ApplicationBuilder>>();
        RequestDelegate app = context => {
            if (context.Handler != null) return context.Handler.Handle(context.Request,
                new ServerStreamWriter(
                    ApplicationServices.GetRequiredService<ISerializer>(),
                    context.Response),
                context.RequestAborted);
            logger?.LogWarning("未找到对应终结点:{endpoints}的处理器", context.Request.Endpoint);
            throw new RPCProtocolException("未找到对应的处理器");
        };
        for (var c = _components.Count - 1; c >= 0; c--) {
            app = _components[c](app);
        }
        return app;
    }
}