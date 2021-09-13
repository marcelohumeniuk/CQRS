using CQRS.Application.Interfaces;
using CQRS.Application.Services;
using CQRS.Domain.Commands;
using CQRS.Domain.Core.Events;
using CQRS.Domain.Events;
using CQRS.Domain.Interfaces;
using CQRS.Infra.CrossCutting.Bus;
using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using CQRS.Infra.Data.Context;
using CQRS.Infra.Data.EventSourcing;
using CQRS.Infra.Data.Repository;
using CQRS.Infra.Data.Repository.EventSourcing;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;

namespace CQRS.Infra.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // DOMAIN BUS (MEDIATOR)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // APPLICATION
            services.AddScoped<ICustomerAppService, CustomerAppService>();

            // DOMAIN - EVENTS
            services.AddScoped<INotificationHandler<CustomerRegisteredEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerUpdatedEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerRemovedEvent>, CustomerEventHandler>();

            // DOMAIN - COMMANDS
            services.AddScoped<IRequestHandler<RegisterNewCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveCustomerCommand, ValidationResult>, CustomerCommandHandler>();

            // INFRA - DATA
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<CQRSContext>();

            // INFRA - DATA EVENTSOURCING
            services.AddScoped<IEventStoreRepository, EventStoreSqlRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSqlContext>();


            services.AddSingleton<ServiceBusProducer>();

            // Infra - Queue RabbitMQ
            //services.AddSingleton<IProducerMessageService, MessageService>();
            //services.AddHostedService<ConsumingQueueService>();


        }
    }
}