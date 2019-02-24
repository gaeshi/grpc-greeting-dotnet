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
            await CallClientStreamingAsync(greetServiceClient);
            await CallBiDiStreamingAsync(greetServiceClient);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CallBiDiStreamingAsync(GreetService.GreetServiceClient greetServiceClient)
        {
            var requests = GreetingGenerator<GreetEveryoneRequest>.GenerateRequests();
            var call = greetServiceClient.GreetEveryone();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    Console.WriteLine($"Reply: {call.ResponseStream.Current.Result}");
                }
            });

            foreach (var greetEveryoneRequest in requests)
            {
                await call.RequestStream.WriteAsync(greetEveryoneRequest);
            }

            await call.RequestStream.CompleteAsync();
            await responseReaderTask;
            Console.WriteLine("Done!");
        }

        private static async Task CallClientStreamingAsync(GreetService.GreetServiceClient greetServiceClient)
        {
            var requests = GreetingGenerator<LongGreetRequest>.GenerateRequests();
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