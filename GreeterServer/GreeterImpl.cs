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
    }
}