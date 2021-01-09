using GrpcService.Services;
using GrpcShared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GrpcService.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class GreeterController : ControllerBase
	{
		public GreeterController(IGreeterService greeterService)
		{
			GreeterService = greeterService;
		}

		public IGreeterService GreeterService { get; }

		[HttpPost]
		public Task<HelloReply> SayHello(HelloRequest request)
			=> GreeterService.SayHelloAsync(request);
	}
}
