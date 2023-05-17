namespace RPC.Server.Core; 

/// <summary>
/// 处理RPC请求的委托
/// </summary>
public delegate Task RequestDelegate(RPCRequestContext context);