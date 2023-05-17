using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using RPC.Abstracts;
using RPC.Abstracts.Interface;

namespace RPC.Server; 

/// <summary>
/// 会话上下文
/// </summary>
public sealed class ServerStreamWriter{
    private readonly PipeWriter _writer;
    private readonly ISerializer _serializer;
    internal ServerStreamWriter(ISerializer serializer, PipeWriter writer) {
        _serializer = serializer;
        _writer = writer;
    }
    public async Task WriteAsync<T>(string requestId, T value) where T : class{
        //  requestId & 响应体长度 & 响应体内容
        ArrayBufferWriter<byte> writer = new();
        byte[] bodies = _serializer.Serialize(value);
        //requestId
        writer.Write(Encoding.ASCII.GetBytes(requestId));
        writer.Write(Constants.Separator);
        //消息体长度
        writer.Write(BitConverter.GetBytes(bodies.Length));
        writer.Write(Constants.Separator);
        writer.Write(bodies);
        //终结符
        writer.Write(Constants.CRLF);
        await _writer.WriteAsync(writer.WrittenMemory);
    }
}