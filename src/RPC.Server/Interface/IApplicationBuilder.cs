using RPC.Server.Core;
using RPC.Server.Internal;

namespace RPC.Server.Interface; 

public interface IApplicationBuilder {
    IServiceProvider ApplicationServices { get; set; }
    IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);
    IApplicationBuilder Use(RequestDelegate middleware);
    IApplicationBuilder Use<TMiddleware>() where TMiddleware : IMiddleware;
    IApplicationBuilder Use<TMiddleware>(TMiddleware middleware) where TMiddleware : IMiddleware;
    IApplicationBuilder Use(Func<RequestDelegate, RPCRequestContext, Task> middleware);
    RequestDelegate Build();
}