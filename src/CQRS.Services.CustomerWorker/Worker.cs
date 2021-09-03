using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Services.CustomerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _serviceBusClient;

        public Worker(ILogger<Worker> logger, ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
        }


        public async Task ReceivedMessageOcorrencia()
        {
            string queueName = "ocorrencia";
           
            ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(queueName);
           
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();

            // get the message body as a string
            string body = receivedMessage.Body.ToString();
            Console.WriteLine(body);

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
           
        }
    }
}
