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
       
        private readonly ILogger _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor processor;
        

        public Worker(ILogger<Worker> logger, 
            ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
           
        }

        public async Task ReceivedMessageOcorrencia(CancellationToken stoppingToken)
        {
            string queueName = "ocorrencia";

            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            processor = _serviceBusClient.CreateProcessor(queueName, options);

            try
            {
                // CONFIGURE THE MESSAGE AND ERROR HANDLER TO USE
                processor.ProcessMessageAsync += ProcessMessagesAsync;
                processor.ProcessErrorAsync += ProcessErrorAsync;
                await processor.StartProcessingAsync(stoppingToken);

                Console.ReadKey();

                // HANDLE RECEIVED MESSAGES
                static async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
                {
                    string body = args.Message.Body.ToString();
                    Console.WriteLine($"Received: {body}");

                    // COMPLETE THE MESSAGE. MESSAGES IS DELETED FROM THE QUEUE. 
                    await args.CompleteMessageAsync(args.Message);
                }

                static Task ProcessErrorAsync(ProcessErrorEventArgs arg)
                {
                    //_logger.LogError(arg.Exception, "Message handler encountered an exception");
                    //_logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
                    //_logger.LogDebug($"- Entity Path: {arg.EntityPath}");
                    //_logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");
                    Console.WriteLine(arg.Exception.ToString());
                    return Task.CompletedTask;
                }
               
            }
            catch (ServiceBusException ex)
            when
                (ex.Reason == ServiceBusFailureReason.ServiceTimeout)
            {
                // Take action based on a service timeout
            }


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await ReceivedMessageOcorrencia(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

        }
    }
}
