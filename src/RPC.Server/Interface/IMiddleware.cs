using RPC.Server.Core;

namespace RPC.Server.Interface; 

public interface IMiddleware {
    Task Invoke(RequestDelegate next,RPCRequestContext context);
}