using AutoMapper;
using Azure.Messaging.ServiceBus;
using CQRS.Application.ViewModels;
using CQRS.Domain.Commands;
using CQRS.Domain.Core;
using CQRS.Domain.Interfaces;
using CQRS.Domain.Models;
using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Services.CustomerWorker
{
    public class OcorrenciaWorker : BackgroundService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor processor;
        private readonly ICustomerRepository _customerRepository;


        public OcorrenciaWorker(ILogger<OcorrenciaWorker> logger, IMapper mapper,
            ServiceBusClient serviceBusClient, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _serviceBusClient = serviceBusClient;
            _mapper = mapper;

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

            try
            {
                // CONFIGURE THE MESSAGE AND ERROR HANDLER TO USE
                processor.ProcessMessageAsync += ProcessMessagesAsync;
                processor.ProcessErrorAsync += ProcessErrorAsync;
                await processor.StartProcessingAsync(stoppingToken);

                Console.ReadKey();

                // HANDLE RECEIVED MESSAGES .....
                static async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
                {
                    string body = args.Message.Body.ToString();

                    //ResolveReceivedQueueToBD(body);

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

        private void ResolveReceivedQueueToBD(string body)
        {

            string[] keys = new string[] { "RegisterNewCustomerCommand", "UpdateCustomerCommand", "RemoveCustomerCommand" };
            string sKeyResult = keys.FirstOrDefault<string>(s => body.Contains(s));

            switch (sKeyResult)
            {
                case "RegisterNewCustomerCommand":
                    var messsageNew = JsonSerializer.Deserialize<RegisterNewCustomerCommand>(body);
                    var customerMapperNew = _mapper.Map<Customer>(messsageNew);
                    _customerRepository.Add(customerMapperNew, TypeDB.StoreRead);
                    Console.WriteLine($"Received: {messsageNew}");
                    break;

                case "UpdateCustomerCommand":
                    var messsageUpdate = JsonSerializer.Deserialize<UpdateCustomerCommand>(body);
                    var customerMapperUpdate = _mapper.Map<Customer>(messsageUpdate);
                    _customerRepository.Update(customerMapperUpdate, TypeDB.StoreRead);
                    Console.WriteLine($"Received: {messsageUpdate}");
                    break;
                case "RemoveCustomerCommand":
                    var messsageRemove = JsonSerializer.Deserialize<RemoveCustomerCommand>(body);
                    var customerMapperRemove = _mapper.Map<Customer>(messsageRemove);
                    _customerRepository.Update(customerMapperRemove, TypeDB.StoreRead);
                    Console.WriteLine($"Received: {messsageRemove}");
                    break;
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
