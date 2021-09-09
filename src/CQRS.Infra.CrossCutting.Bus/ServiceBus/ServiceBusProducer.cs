using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CQRS.Infra.CrossCutting.Bus.ServiceBus
{
    public class ServiceBusProducer
    {
        private readonly ServiceBusClient serviceBusClient;

        public ServiceBusProducer(ServiceBusClient serviceBusClient)
        {
            this.serviceBusClient = serviceBusClient;
        }

        public async Task SendSBMessage(string queueName, object message)
        {
                // CREATE THE SENDER
                ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

                var serializedMessage = JsonConvert.SerializeObject(message);

                // CREATE A MESSAGE THAT WE CAN SEND. UTF-8 ENCODING IS USED WHEN PROVIDING A STRING.
                ServiceBusMessage messageSender = new ServiceBusMessage(serializedMessage);           
                await sender.SendMessageAsync(messageSender);

        }
    }
}
