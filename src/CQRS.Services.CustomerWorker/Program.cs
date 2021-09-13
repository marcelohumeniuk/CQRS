using AutoMapper;
using CQRS.Application.AutoMapper;
using CQRS.Application.Interfaces;
using CQRS.Application.Services;
using CQRS.Domain.Interfaces;
using CQRS.Infra.CrossCutting.IoC;
using CQRS.Infra.Data.Context;
using CQRS.Infra.Data.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDevPack.Identity.User;

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


                    services.AddDbContext<CQRSContext>(options =>
                             options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddDbContext<CQRSRead_Context>(options =>
                       options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnectionRead")));

                    services.AddDbContext<EventStoreSqlContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));


                    services.AddDbContext<EventStoreSqlContext>(options =>
                       options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddAspNetUserConfiguration();

                    // AUTOMAPPER SETTINGS
                    services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));

                    // ADDING MEDIATR FOR DOMAIN EVENTS AND NOTIFICATIONS
                    services.AddMediatR(typeof(Program));

                    NativeInjectorBootStrapper.RegisterServices(services);



                    services.AddAzureClients(builder =>
                    {
                        builder.AddServiceBusClient(hostContext.Configuration.GetConnectionString("ServiceBus"));
                    });


                });
    }
}



