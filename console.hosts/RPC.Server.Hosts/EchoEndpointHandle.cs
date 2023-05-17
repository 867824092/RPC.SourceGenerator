using RPC.Abstracts;
using RPC.Server.Interface;

namespace RPC.Server.Hosts; 

public class EchoEndpointHandle : IEndpointHandle {
    public string Endpoint { get; } = "Echo";
    public async Task Handle(RPCRequest request,ServerStreamWriter stream,CancellationToken cancellationToken = default) {
        Console.WriteLine("EchoEndpointHandle");
        await Task.Delay(100, cancellationToken);
        await stream.WriteAsync(request.RequestId, "123");
    }
}