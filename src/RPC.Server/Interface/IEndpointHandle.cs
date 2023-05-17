using RPC.Abstracts;

namespace RPC.Server.Interface; 

public interface IEndpointHandle {
    string Endpoint { get; }
    Task Handle(RPCRequest request,ServerStreamWriter stream ,CancellationToken cancellationToken = default);
}