using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using RPC.Abstracts;
using RPC.Abstracts.Interface;
using RPC.Server.Core;
using Constants = RPC.Abstracts.Constants;

namespace RPC.Server.Internal; 

public sealed class TcpConnectionHandler : ConnectionHandler {
    private readonly ILogger<TcpConnectionHandler> _logger;
    public RequestDelegate? RequestDelegate { get; set; }
    public TcpConnectionHandler(
        ILoggerFactory factory) {
        _logger = factory.CreateLogger<TcpConnectionHandler>();
    }
    public override async Task OnConnectedAsync(ConnectionContext connection) {
        if(RequestDelegate == null) throw new InvalidOperationException("未设置RequestDelegate");
        _logger.LogTrace("Connection {connectionId} connected.", connection.ConnectionId);
        //await connection.Transport.Output.WriteAsync(Encoding.UTF8.GetBytes($"Hello:{connection.ConnectionId}"));
        while (!connection.ConnectionClosed.IsCancellationRequested) {
            var result = await connection.Transport.Input.ReadAsync();
            var buffer = result.Buffer;
            try {
                if (!buffer.IsEmpty) {
                   var requests = TryParse(ref buffer, out SequencePosition consumed);
                   if (requests.Count > 0) {
                       foreach (RPCRequest request in requests) {
                           await RequestDelegate.Invoke(
                               new RPCRequestContext(connection.ConnectionId, request, connection.RemoteEndPoint!
                                   ,connection.Transport.Output)
                               { RequestAborted = connection.ConnectionClosed });
                       }
                   }
                } else
                    if (result.IsCompleted) {
                        break;
                    }
            }
            finally {
                connection.Transport.Input.AdvanceTo(buffer.End);
            }
        }
        _logger.LogTrace("Connection {connectionId} disconnected.", connection.ConnectionId);
    }

    private List<RPCRequest> TryParse(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed) {
        //读取buffer里的内容，判断是否有换行符
        consumed = buffer.Start;
        var reader = new SequenceReader<byte>(buffer);
        List<RPCRequest> requests = new();
        while (!reader.End) {
            if (reader.TryReadTo(out ReadOnlySpan<byte> line, Constants.CRLF)) {
                if (line.Length < 4) {
                    throw new RPCProtocolException();
                }
                //  endpoint & requestId &  请求长度 & 请求内容  \r\n
                var package = new DataPackage(line);
                string endpoint = package.ReadString(0, package.IndexOf(Constants.Separator));
                package.Advance(Constants.Separator.Length);
                string requestId = package.ReadString(package.Offset, package.IndexOf(Constants.Separator));
                package.Advance(Constants.Separator.Length);
                int bodyLength = package.ReadInt32(package.Offset, package.IndexOf(Constants.Separator));
                package.Advance(Constants.Separator.Length);
                if (bodyLength < 0) {
                    throw new RPCProtocolException("请求体长度不能小于0");
                }
                byte[] bodies = package.Read(package.Offset, bodyLength).ToArray();
                requests.Add(new RPCRequest(endpoint, requestId, bodies));
            } else {
                //没有读取到换行符，等待下一次读取
                break;
            }
        }

        consumed = reader.Position;
        return requests;
    }
}

public ref struct DataPackage {
    private ReadOnlySpan<byte> _buffer;
    public DataPackage(ReadOnlySpan<byte> buffer) {
        _buffer = buffer;
        _offset = 0;
    }
    private int _offset;

    public int Offset {
        get => _offset;
        set {
            if(value > _buffer.Length) throw new IndexOutOfRangeException();
            _offset = value;
        }
    }

    public int ReadInt32(int start, int length) {
        return BitConverter.ToInt32(Read(start, length));
    }
    public string ReadString(int start,int length) {
        return Encoding.ASCII.GetString(Read(start, length));
    }

    public ReadOnlySpan<byte> Read(int start) {
        return _buffer[_offset..][start..];
    }
    public ReadOnlySpan<byte> Read(int start,int length) {
        var span = _buffer.Slice(start, length);
        _offset += length;
        return span;
    }
    public void Advance(int count) {
        _offset += count;
    }
    public int IndexOf(Span<byte> value) {
        return _buffer[_offset..].IndexOf(value);
    }
}