using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace CQRS.Services.CustomerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        private static IConfiguration Configuration { get; set; }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AppConfiguration)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddAzureClients(builder =>
                    {
                        builder.AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"));
                    });
                });

        //TODO: VOLTAR E VERIFICAR VARIAVEL DE AMBIENTE
        private static void AppConfiguration(IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.Development.json");

            Configuration = config.Build();
        }
    }
}



