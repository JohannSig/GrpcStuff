using GrpcShared;
using System.Threading.Tasks;

namespace GrpcService.Services
{
	public interface IGreeterService
	{
		Task<HelloReply> SayHelloAsync(HelloRequest request);
	}

	public class GreeterService : IGreeterService 
	{
		public Task<HelloReply> SayHelloAsync(HelloRequest request)
		=> Task.FromResult(new HelloReply
		{
			Message = "Hello " + request.Name
		});
	}
}
