using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using CQRS.Infra.CrossCutting.Identity;
using CQRS.Services.Api.Configurations;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDevPack.Identity;
using NetDevPack.Identity.User;

namespace CQRS.Services.Api
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecks()  //.AddServiceBusClient()


           .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"),
               name: "sqlserver", tags: new string[] { "db", "data" });

            services.AddHealthChecksUI()
                .AddInMemoryStorage();



            services.AddAzureClients(builder =>
                {
                    builder.AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"));
                });


            //services.AddSingleton<OcorrenciaProducer>();
            services.AddSingleton<ServiceBusProducer>();

            // WebAPI Config
            services.AddControllers();

            // Setting DBContexts
            services.AddDatabaseConfiguration(Configuration);

            // ASP.NET Identity Settings & JWT
            services.AddApiIdentityConfiguration(Configuration);

            // Interactive AspNetUser (logged in)
            // NetDevPack.Identity dependency
            services.AddAspNetUserConfiguration();

            // AutoMapper Settings
            services.AddAutoMapperConfiguration();

            // Swagger Config
            services.AddSwaggerConfiguration();

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            // .NET Native DI Abstraction
            services.AddDependencyInjectionConfiguration();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            // NetDevPack.Identity dependency
            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerSetup();

            // SABER STATUS POR JSON
            //app.UseHealthChecks("/status-json",
            //    new HealthCheckOptions()
            //    {
            //        ResponseWriter = async (context, report) =>
            //        {
            //            var result = JsonSerializer.Serialize(
            //                new
            //                {
            //                    currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //                    statusApplication = report.Status.ToString(),
            //                });

            //            context.Response.ContentType = MediaTypeNames.Application.Json;
            //            await context.Response.WriteAsync(result);
            //        }
            //    });


            // GERA O ENDPOINT QUE RETORNARÁ OS DADOS UTILIZADOS NO DASHBOARD
            app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // ATIVA O DASHBOARD PARA A VISUALIZAÇÃO DA SITUAÇÃO DE CADA HEALTH CHECK
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/monitor";
            });



        }
    }
}
