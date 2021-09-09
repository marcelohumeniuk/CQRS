using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CQRS.Infra.CrossCutting.Bus.ServiceBus
{
    public class ServiceBusProducer
    {
        private readonly ILogger _logger;
        private readonly ServiceBusClient serviceBusClient;

        public ServiceBusProducer(ServiceBusClient serviceBusClient, ILogger logger)
        {
            _logger = logger;
            this.serviceBusClient = serviceBusClient;
        }

        public async Task SendSBMessage(string queueName, object message)
        {
            try
            {
                // CREATE THE SENDER
                ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

                var serializedMessage = JsonConvert.SerializeObject(message);

                // CREATE A MESSAGE THAT WE CAN SEND. UTF-8 ENCODING IS USED WHEN PROVIDING A STRING.
                ServiceBusMessage messageSender = new ServiceBusMessage(serializedMessage);
                await sender.SendMessageAsync(messageSender);
            }
            catch (ServiceBusException ex)
          when (ex.Reason == ServiceBusFailureReason.ServiceTimeout)
            {
                _logger.LogInformation("ServiceBusFailureReason", ex.Data);
                // Take action based on a service timeout
            }


        }
    }
}
