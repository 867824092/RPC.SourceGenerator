namespace RPC.Server; 

public sealed class RPCProtocolException : Exception {
    public RPCProtocolException():this("协议格式错误") {
        
    }
    public RPCProtocolException(string message):base(message) {
        
    }

}