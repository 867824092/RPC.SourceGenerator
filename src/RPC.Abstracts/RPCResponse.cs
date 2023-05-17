namespace RPC.Abstracts; 

public class RPCResponse {
    public string RequestId { get; set; } = null!;
    public byte[] Bodies { get; set; } = null!;
}