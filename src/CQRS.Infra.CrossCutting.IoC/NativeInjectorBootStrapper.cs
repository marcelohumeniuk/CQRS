using CQRS.Application.Interfaces;
using CQRS.Application.Services;
using CQRS.Domain.Commands;
using CQRS.Domain.Core.Events;
using CQRS.Domain.Events;
using CQRS.Domain.Interfaces;
using CQRS.Infra.CrossCutting.Bus;
using CQRS.Infra.CrossCutting.RabbitMQ.Consumer;
using CQRS.Infra.CrossCutting.RabbitMQ.Producer;
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
            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Application
            services.AddScoped<ICustomerAppService, CustomerAppService>();

            // Domain - Events
            services.AddScoped<INotificationHandler<CustomerRegisteredEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerUpdatedEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerRemovedEvent>, CustomerEventHandler>();

            // Domain - Commands
            services.AddScoped<IRequestHandler<RegisterNewCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveCustomerCommand, ValidationResult>, CustomerCommandHandler>();

            // Infra - Data
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<CQRSContext>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreSqlRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSqlContext>();

            // Infra - Queue RabbitMQ
            services.AddSingleton<IMessageService, MessageService>();
           
            services.AddHostedService<ConsumingQueueService>();


        }
    }
}