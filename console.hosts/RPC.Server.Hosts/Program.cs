using System.Net;
using Microsoft.AspNetCore.Connections;
using RPC.Server;
using RPC.Server.Hosts;
using RPC.Server.Interface;
using RPC.Server.Internal;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => {
    options.Listen(IPAddress.Any, 5000, listenOptions => {
        listenOptions.UseConnectionHandler<TcpConnectionHandler>();
    });
});
builder.Services.AddTransient<IEndpointHandle, EchoEndpointHandle>();
builder.Services.AddRPCServer();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();