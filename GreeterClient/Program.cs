using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace GreeterClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var greetServiceClient = new GreetService.GreetServiceClient(channel);
            var greeting = new Greeting {FirstName = "Sheldon", LastName = "Cooper"};

            CallUnary(greetServiceClient, new GreetRequest {Greeting = greeting});
            await CallServerStreamingAsync(greetServiceClient, new GreetManyTimesRequest {Greeting = greeting});

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CallServerStreamingAsync(GreetService.GreetServiceClient greetServiceClient,
            GreetManyTimesRequest request)
        {
            var asyncServerStreamingCall = greetServiceClient.GreetManyTimes(request);
            var responseStream = asyncServerStreamingCall.ResponseStream;
            while (await responseStream.MoveNext())
            {
                var result = responseStream.Current;
                Console.WriteLine($"Response: {result.Result}");
            }

            Console.WriteLine("Done!");
        }

        private static void CallUnary(GreetService.GreetServiceClient greetServiceClient, GreetRequest greetRequest)
        {
            var reply = greetServiceClient.Greet(greetRequest);
            Console.WriteLine($"Greeting: {reply.Result}");
        }
    }
}