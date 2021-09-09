using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CQRS.Services.Api
{
    [Obsolete]
    public class OcorrenciaProducer
    {
        private readonly ServiceBusClient serviceBusClient;

        public OcorrenciaProducer(ServiceBusClient serviceBusClient)
        {
            this.serviceBusClient = serviceBusClient;
        }

        public async Task SendMessageOcorrencia() {
            
            string queueName = "ocorrencia";


            try
            {
                // CREATE THE SENDER
                ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

                var mg = "";
                var serializedMessage = JsonConvert.SerializeObject(mg);
                // CREATE A MESSAGE THAT WE CAN SEND. UTF-8 ENCODING IS USED WHEN PROVIDING A STRING.
                ServiceBusMessage message = new ServiceBusMessage(serializedMessage);            // send the message
                await sender.SendMessageAsync(message);

            }
            catch (Exception ex)
            {
               
                throw;
            }
           
         

        }






    }
}
