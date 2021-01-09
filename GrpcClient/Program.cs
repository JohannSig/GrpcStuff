using Grpc.Net.Client;
using GrpcShared;
using GrpcShared.MessagePackResolvers;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GrpcClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var measurements = new Dictionary<string, List<long>>
			{
				["gRPC (native)"] = new List<long>(),
				["gRPC (REST)"] = new List<long>(),
				["Controller (JSON)"] = new List<long>(),
				["Controller (MSGPACK)"] = new List<long>()
			};

			var request = new HelloRequest { Name = "GreeterClient" };

			var sw = Stopwatch.StartNew();

			// Using gRPC client:
			using var channel = GrpcChannel.ForAddress("https://localhost:5001");
			var client = new Greeter.GreeterClient(channel);
			sw.Restart();

			for (var i = 0; i < 1000; i++)
			{
				var reply = await client.SayHelloAsync(request);
				var measurement = sw.ElapsedMilliseconds;
				if (i > 1) measurements["gRPC (native)"].Add(measurement);
				Console.WriteLine($"gRPC (native) {i}: {reply.Message} ({measurement}ms)");
				sw.Restart();
			}

			// Using HttpClient(s):
			using var httpClient = new HttpClient();
			sw.Restart();

			// Using gRPC endpoint over REST
			for (var i = 1; i <= 1000; i++)
			{
				using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://localhost:5001/v1/greeter/{request.Name}")
				{
					Content = JsonContent.Create(request),
					Version = HttpVersion.Version20
				};

				using var response = await httpClient
					.SendAsync(httpRequestMessage)
					.ConfigureAwait(false);

				var reply = await response.Content
					.ReadFromJsonAsync<HelloReply>()
					.ConfigureAwait(false);

				var measurement = sw.ElapsedMilliseconds;
				if (i > 1) measurements["gRPC (REST)"].Add(measurement);
				Console.WriteLine($"gRPC (REST) {i}: {reply.Message} ({measurement}ms)");
				sw.Restart();
			}

			// Using API controller (POST, JSON)
			for (var i = 1; i <= 1000; i++)
			{
				using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/api/Greeter/SayHello")
				{
					Content = JsonContent.Create(request),
					Version = HttpVersion.Version20
				};

				var response = await httpClient
					.SendAsync(httpRequestMessage)
					.ConfigureAwait(false);

				var reply = await response.Content
					.ReadFromJsonAsync<HelloReply>()
					.ConfigureAwait(false);

				var measurement = sw.ElapsedMilliseconds;
				if (i > 1) measurements["Controller (JSON)"].Add(measurement);
				Console.WriteLine($"Controller (JSON) {i}: {reply.Message} ({measurement})ms");
				sw.Restart();
			}

			for (var i = 1; i <= 1000; i++)
			{
				using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/api/Greeter/SayHello")
				{
					Content = JsonContent.Create(request),
					Version = HttpVersion.Version20,
				};

				httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-msgpack"));

				var response = await httpClient
					.SendAsync(httpRequestMessage)
					.ConfigureAwait(false);

				var stream = await response.Content
					.ReadAsStreamAsync()
					.ConfigureAwait(false);

				var reply = await MessagePackSerializer
					.DeserializeAsync<HelloReply>(stream, CustomResolver.Options)
					.ConfigureAwait(false);

				var measurement = sw.ElapsedMilliseconds;
				if (i > 1) measurements["Controller (MSGPACK)"].Add(measurement);
				Console.WriteLine($"Controller (MSGPACK) {i}: {reply.Message} ({measurement})ms");
				sw.Restart();
			}

			Console.WriteLine($"Summary: \n{string.Join('\n', measurements.Select(x => $"{x.Key}: {x.Value.Average()}ms (avg)"))}");

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
