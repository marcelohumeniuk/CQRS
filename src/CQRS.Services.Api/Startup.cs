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

            services.AddHealthChecks()  //.AddServiceBusClient<ServiceBusClient>

           .AddSqlServer( Configuration.GetConnectionString("DefaultConnection"),
               name: "sqlserver", tags: new string[] { "db", "data" });

            services.AddHealthChecksUI()
                .AddInMemoryStorage();


            // CONFIG CONNECTION SB AZURE
            services.AddAzureClients(builder =>
                {
                    builder.AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"));
                });


            //services.AddSingleton<OcorrenciaProducer>();

          

            // WEBAPI CONFIG
            services.AddControllers();

            // SETTING DBCONTEXTS
            services.AddDatabaseConfiguration(Configuration);

            // ASP.NET IDENTITY SETTINGS & JWT
            services.AddApiIdentityConfiguration(Configuration);

            // INTERACTIVE ASPNETUSER (LOGGED IN)
            // NETDEVPACK.IDENTITY DEPENDENCY
            services.AddAspNetUserConfiguration();

            // AUTOMAPPER SETTINGS
            services.AddAutoMapperConfiguration();

            // SWAGGER CONFIG
            services.AddSwaggerConfiguration();

            // ADDING MEDIATR FOR DOMAIN EVENTS AND NOTIFICATIONS
            services.AddMediatR(typeof(Startup));

            // .NET NATIVE DI ABSTRACTION
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

            // NETDEVPACK.IDENTITY DEPENDENCY
            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerSetup();


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
