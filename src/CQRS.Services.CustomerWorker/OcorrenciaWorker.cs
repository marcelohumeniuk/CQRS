using Azure.Messaging.ServiceBus;
using CQRS.Application.Interfaces;
using CQRS.Application.Services;
using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Services.CustomerWorker
{
    public class OcorrenciaWorker : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor processor;
        //private readonly ICustomerAppService _customerAppService;
        private IServiceProvider _serviceProvider;


        public OcorrenciaWorker(ILogger<OcorrenciaWorker> logger,
            //ICustomerAppService customerAppService,
            IServiceProvider serviceProvider,

            ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            //_customerAppService = customerAppService;
            _serviceProvider = serviceProvider;


        }

        public async Task ReceivedMessageOcorrencia(CancellationToken stoppingToken)
        {
            string queueName = QueueName.Ocorrencia;

            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = true,
            };

            processor = _serviceBusClient.CreateProcessor(queueName, options);

            // CONFIGURE THE MESSAGE AND ERROR HANDLER TO USE
            processor.ProcessMessageAsync += ProcessMessagesAsync;
            processor.ProcessErrorAsync += ProcessErrorAsync;
            await processor.StartProcessingAsync(stoppingToken);

            Console.ReadKey();

        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {

            try
            {
                string body = args.Message.Body.ToString();
                _logger.LogInformation(body);

                using (var scope = _serviceProvider.CreateScope()) 
                {

                    var appService = scope.ServiceProvider.GetService<ICustomerAppService>();
                    appService.ResolveReceivedQueueToBD(body);

                }

                // COMPLETE THE MESSAGE. MESSAGES IS DELETED FROM THE QUEUE. 
                await args.CompleteMessageAsync(args.Message);
            }
            catch (ServiceBusException ex)
              when
               (ex.Reason == ServiceBusFailureReason.ServiceTimeout)
            {
                // Take action based on a service timeout
            }


        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Message handler encountered an exception");
            _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
            _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
            _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
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
