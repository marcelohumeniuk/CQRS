using MediatR;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Domain.Events
{
    public class CustomerEventHandler : 
        INotificationHandler<CustomerRegisteredEvent>, 
        INotificationHandler<CustomerUpdatedEvent>, 
        INotificationHandler<CustomerRemovedEvent>
    {

        // SERVICE RABBITMQ
        private readonly IProducerMessageService _messageService;


        public CustomerEventHandler()
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