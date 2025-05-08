using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using SampleStack.Api.HostedServices;
using SampleStack.Api.Providers;

namespace SampleStack.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHostedService<SocketHostedService>();
            builder.Services.AddSingleton<ISocketDataProvider, InMemorySocketDataProvider>();
            builder.Services.AddMemoryCache();
            builder.Services.AddOptions();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen((setup) => {
                setup.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Network Communications API",
                    Description = "Provides sample endpoints to demonstrate network communications.",
                    Contact = new OpenApiContact()
                    {
                        Name = "David Kyle (RoviSys)",
                        Email = "david.kyle@rovisys.com",
                        Extensions = new Dictionary<string, IOpenApiExtension>() { { "x-Company", new OpenApiString("The RoviSys Company") } },
                        Url = new Uri("https://www.rovisys.com")
                    }
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "custom",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:4200");
                                  });
            });
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableValidator();
                options.EnableDeepLinking();
                options.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>()
                {
                    ["activated"] = false
                };
            });

            app.UseRouting();
            app.UseCors("custom");

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}
