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


        public async Task ReceivedMessageOcorrencia(CancellationToken stoppingToken)
        {
            string queueName = "ocorrencia";

            //ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(queueName);

            //ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();

            //// get the message body as a string
            //string body = receivedMessage.Body.ToString();
            //Console.WriteLine(body);


            try
            {
                var options = new ServiceBusProcessorOptions
                {
                    // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                    // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                    // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                    AutoCompleteMessages = false,

                    // I can also allow for multi-threading
                    MaxConcurrentCalls = 2
                };

                // create a processor that we can use to process the messages
                await using ServiceBusProcessor processor = _serviceBusClient.CreateProcessor(queueName, options);

                // configure the message and error handler to use
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                async Task MessageHandler(ProcessMessageEventArgs args)
                {
                    string body = args.Message.Body.ToString();
                    Console.WriteLine(body);

                    // we can evaluate application logic and use that to determine how to settle the message.
                    await args.CompleteMessageAsync(args.Message);
                }

                Task ErrorHandler(ProcessErrorEventArgs args)
                {
                    // the error source tells me at what point in the processing an error occurred
                    Console.WriteLine(args.ErrorSource);
                    // the fully qualified namespace is available
                    Console.WriteLine(args.FullyQualifiedNamespace);
                    // as well as the entity path
                    Console.WriteLine(args.EntityPath);
                    Console.WriteLine(args.Exception.ToString());
                    return Task.CompletedTask;
                }

                // start processing
                await processor.StartProcessingAsync(stoppingToken);
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


            //await ReceivedMessageOcorrencia();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await ReceivedMessageOcorrencia(stoppingToken);
                    //await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

        }
    }
}
