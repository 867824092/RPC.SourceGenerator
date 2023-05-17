namespace RPC.Abstracts; 

public class RPCRequest {
    public string Endpoint { get;  }
    public string RequestId { get;  }
    public byte[] Bodies { get; }

    public RPCRequest(string endpoint,string requestId,byte[] bodies) {
        Endpoint = endpoint;
        RequestId = requestId;
        Bodies = bodies;
    }
}