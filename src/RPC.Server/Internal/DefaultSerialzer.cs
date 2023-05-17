using System.Text;
using RPC.Abstracts.Interface;

namespace RPC.Server.Internal; 

internal sealed class DefaultSerialzer : ISerializer {
    public byte[] Serialize<T>(T value) where T : class {
        return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(value));
    }

    public T? Deserialize<T>(byte[] bytes) where T : class {
        return System.Text.Json.JsonSerializer.Deserialize<T>(bytes);
    }
}