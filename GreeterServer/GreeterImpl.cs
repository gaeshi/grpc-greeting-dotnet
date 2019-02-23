using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace GreeterServer
{
    class GreeterImpl : GreetService.GreetServiceBase
    {
        public override Task<GreetResponse> Greet(GreetRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GreetResponse {Result = $"Hello, {request.Greeting.FirstName}"});
        }

        public override async Task GreetManyTimes(GreetManyTimesRequest request,
            IServerStreamWriter<GreetManyTimesResponse> responseStream, ServerCallContext context)
        {
            var firstName = request.Greeting.FirstName;

            for (var i = 0; i < 10; i++)
            {
                var result = $"Hello {firstName}, response number: {i}";
                await responseStream.WriteAsync(new GreetManyTimesResponse {Result = result});
                Thread.Sleep(1000);
            }
        }

        public override async Task<LongGreetResponse> LongGreet(
            IAsyncStreamReader<LongGreetRequest> requestStream,
            ServerCallContext context)
        {
            var names = new List<string>();
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                names.Add(requestStream.Current.Greeting.FirstName);
            }

            return new LongGreetResponse {Result = $"Hello, {string.Join(", ", names)}!"};
        }
    }
}