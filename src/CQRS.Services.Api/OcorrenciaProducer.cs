using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // since ServiceBusClient implements IAsyncDisposable we create it with "await using"
            //await using var client = new ServiceBusClient(serviceBusClient. connectionString);

            // create the sender
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            // create a message that we can send. UTF-8 encoding is used when providing a string.

            ServiceBusMessage message = new ServiceBusMessage("Hello world!" + DateTime.Now.ToString());            // send the message
            await sender.SendMessageAsync(message);
         

        }






    }
}
