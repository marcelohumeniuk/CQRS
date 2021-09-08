using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace CQRS.Services.Api
{
    public class OcorrenciaProducer
    {
        private readonly ServiceBusClient serviceBusClient;

        public OcorrenciaProducer(ServiceBusClient serviceBusClient)
        {
            this.serviceBusClient = serviceBusClient;
        }

        public async Task SendMessageOcorrencia() {
            
            string queueName = "ocorrencia";

            // CREATE THE SENDER
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            // CREATE A MESSAGE THAT WE CAN SEND. UTF-8 ENCODING IS USED WHEN PROVIDING A STRING.
            ServiceBusMessage message = new ServiceBusMessage("TESTANDO SEVICE BUS CQRS!" + DateTime.Now.ToString());            // send the message
            await sender.SendMessageAsync(message);
         

        }






    }
}
