// The port number must match the port of the gRPC server.
using Grpc.Net.Client;
using GrpcGreeterClient;
using System.Diagnostics;

var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

using var channel = GrpcChannel.ForAddress("https://localhost:7177",
    new GrpcChannelOptions { HttpHandler = handler });
Stopwatch sw = Stopwatch.StartNew();

for (int i = 0; i < 100; i++)
{
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(
                      new HelloRequest { Name = i.ToString() });
    Console.WriteLine("Greeting: " + reply.Message);
}
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);



