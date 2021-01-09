using GrpcService.gRPC;
using GrpcService.Services;
using GrpcShared.MessagePackResolvers;
using MessagePack.AspNetCoreMvcFormatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GrpcService
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// Controller configuration
			services.AddControllers(config =>
			{
				config.InputFormatters.Add(new MessagePackInputFormatter(CustomResolver.Options));
				config.OutputFormatters.Add(new MessagePackOutputFormatter(CustomResolver.Options));
			});

			// gRPC configuration
			services.AddGrpc();
			services.AddGrpcHttpApi();
			
			// Swagger
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});
			services.AddGrpcSwagger();

			// Application logic
			services.AddScoped<IGreeterService, GreeterService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseRouting();
			app.UseHttpsRedirection();			

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapGrpcService<GreeterRpcService>();
			});
		}
	}
}
