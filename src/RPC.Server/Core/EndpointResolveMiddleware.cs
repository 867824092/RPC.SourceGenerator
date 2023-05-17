using RPC.Abstracts;
using RPC.Server.Interface;

namespace RPC.Server.Core; 

sealed class EndpointResolveMiddleware : IMiddleware  {
    private readonly IEnumerable<IEndpointHandle> _handles;
    public EndpointResolveMiddleware(IEnumerable<IEndpointHandle> handles) {
        _handles = handles;
    }
    public async Task Invoke(RequestDelegate next, RPCRequestContext context) {
        context.Handler = _handles.FirstOrDefault(u => u.Endpoint == context.Request.Endpoint);
       await next(context);
    }
}