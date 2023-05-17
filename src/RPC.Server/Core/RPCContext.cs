using System.IO.Pipelines;
using System.Net;
using RPC.Abstracts;
using RPC.Server.Interface;

namespace RPC.Server.Core; 

public class RPCRequestContext {
    
    public RPCRequestContext(string connectionId,
        RPCRequest request,
        EndPoint remoteEndPoint, 
        PipeWriter response) {
        ConnectionId = connectionId;
        Request = request;
        RemoteEndPoint = remoteEndPoint;
        Response = response;
    }
    public string ConnectionId { get;  }
    public EndPoint? RemoteEndPoint { get;  }
    public RPCRequest Request { get;  }
    public PipeWriter Response { get; }
    public IEndpointHandle? Handler { get; set; }
    public CancellationToken RequestAborted { get; set; }
}