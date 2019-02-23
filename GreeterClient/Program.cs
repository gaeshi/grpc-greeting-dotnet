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
            await CallClientStreamingAsync(greetServiceClient, new List<LongGreetRequest>
            {
                new LongGreetRequest {Greeting = new Greeting {FirstName = "Leonard", LastName = "Hofstadter"}},
                new LongGreetRequest {Greeting = new Greeting {FirstName = "Howard", LastName = "Wolowitz"}},
                new LongGreetRequest {Greeting = new Greeting {FirstName = "Raj", LastName = "Koothrappali"}},
            });

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CallClientStreamingAsync(
            GreetService.GreetServiceClient greetServiceClient,
            IEnumerable<LongGreetRequest> requests)
        {
            var asyncClientStreamingCall = greetServiceClient.LongGreet();
            foreach (var request in requests)
            {
                await asyncClientStreamingCall.RequestStream.WriteAsync(request);
            }

            await asyncClientStreamingCall.RequestStream.CompleteAsync();
            var response = await asyncClientStreamingCall.ResponseAsync;
            Console.WriteLine($"Response: {response.Result}");
            Console.WriteLine("Done!");
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