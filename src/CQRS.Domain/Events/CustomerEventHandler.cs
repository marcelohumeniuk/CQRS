using System.Threading;
using System.Threading.Tasks;
using CQRS.Infra.CrossCutting.RabbitMQ.Producer;
using MediatR;
using Newtonsoft.Json;

namespace CQRS.Domain.Events
{
    public class CustomerEventHandler : 
        INotificationHandler<CustomerRegisteredEvent>, 
        INotificationHandler<CustomerUpdatedEvent>, 
        INotificationHandler<CustomerRemovedEvent>
    {

        // SERVICE RABBITMQ
        private readonly IMessageService _messageService;


        public CustomerEventHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public Task Handle(CustomerUpdatedEvent message, CancellationToken cancellationToken)
        {
            // Send some notification e-mail

            var resultado = JsonConvert.SerializeObject(message);
            _messageService.Enqueue(resultado);

            return Task.CompletedTask;
        }

        public Task Handle(CustomerRegisteredEvent message, CancellationToken cancellationToken)
        {
            // Send some greetings e-mail

            var resultado = JsonConvert.SerializeObject(message);
            _messageService.Enqueue(resultado);

            return Task.CompletedTask;
        }

        public Task Handle(CustomerRemovedEvent message, CancellationToken cancellationToken)
        {
            // Send some see you soon e-mail

            var resultado = JsonConvert.SerializeObject(message);
            _messageService.Enqueue(resultado);

            return Task.CompletedTask;
        }
    }
}