using System;
using Greet;
using Grpc.Core;

namespace GreeterClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var greetServiceClient = new GreetService.GreetServiceClient(channel);
            var greetRequest = new GreetRequest
            {
                Greeting = new Greeting
                {
                    FirstName = "John",
                    LastName = "Snow"
                }
            };

            var reply = greetServiceClient.Greet(greetRequest);

            Console.WriteLine($"Greeting: {reply.Result}");

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}