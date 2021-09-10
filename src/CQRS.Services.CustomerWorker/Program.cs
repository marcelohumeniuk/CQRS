using AutoMapper;
using CQRS.Application.AutoMapper;
using CQRS.Application.Interfaces;
using CQRS.Application.Services;
using CQRS.Domain.Interfaces;
using CQRS.Infra.CrossCutting.IoC;
using CQRS.Infra.Data.Repository;
using MediatR;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CQRS.Services.CustomerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<OcorrenciaWorker>();

                    services.AddAzureClients(builder =>
                    {
                        builder.AddServiceBusClient(hostContext.Configuration.GetConnectionString("ServiceBus"));
                    });


                });
    }
}



