using Grpc.Core;
using GrpcService.Services;
using GrpcShared;
using System.Threading.Tasks;

namespace GrpcService.gRPC
{
	public class GreeterRpcService : Greeter.GreeterBase
	{
	
		public GreeterRpcService(IGreeterService greeterService)
		{
			GreeterService = greeterService;
		}

		public IGreeterService GreeterService { get; }

		public override Task<HelloReply> SayHello(
			HelloRequest request, 
			ServerCallContext context)
			=> GreeterService.SayHelloAsync(request);
	}
}
