namespace RPC.Abstracts.Interface; 

public interface ISerializer {
    byte[] Serialize<T>(T value) where T : class;
    T? Deserialize<T>(byte[] bytes) where T : class;
}