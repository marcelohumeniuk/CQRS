using AutoMapper;
using CQRS.Application.EventSourcedNormalizers;
using CQRS.Application.Interfaces;
using CQRS.Application.ViewModels;
using CQRS.Domain.Commands;
using CQRS.Domain.Core;
using CQRS.Domain.Interfaces;
using CQRS.Domain.Models;
using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using CQRS.Infra.Data.Repository.EventSourcing;
using FluentValidation.Results;
using NetDevPack.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CQRS.Application.Services
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMediatorHandler _mediator;
        private readonly ServiceBusProducer _serviceBusProducer;

        public CustomerAppService(IMapper mapper,
                                  ICustomerRepository customerRepository,
                                  IMediatorHandler mediator,
                                  IEventStoreRepository eventStoreRepository,
                                  ServiceBusProducer serviceBusProducer)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _mediator = mediator;
            _eventStoreRepository = eventStoreRepository;
            _serviceBusProducer = serviceBusProducer;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAll()
        {
            return _mapper.Map<IEnumerable<CustomerViewModel>>(await _customerRepository.GetAll());
        }

        public async Task<CustomerViewModel> GetById(Guid id)
        {
            return _mapper.Map<CustomerViewModel>(await _customerRepository.GetById(id));
        }

        public async Task<ValidationResult> Register(CustomerViewModel customerViewModel)
        {
            var registerCommand = _mapper.Map<RegisterNewCustomerCommand>(customerViewModel);
            var retorno = await _mediator.SendCommand(registerCommand);

            if (retorno.IsValid && !retorno.Errors.Any())
            {
                //await _serviceBusProducer.SendSBMessage(QueueName.Ocorrencia, registerCommand);
            }

            return retorno;
        }

        public async Task<ValidationResult> Update(CustomerViewModel customerViewModel)
        {
            var updateCommand = _mapper.Map<UpdateCustomerCommand>(customerViewModel);
            return await _mediator.SendCommand(updateCommand);
        }

        public async Task<ValidationResult> Remove(Guid id)
        {
            var removeCommand = new RemoveCustomerCommand(id);
            return await _mediator.SendCommand(removeCommand);
        }

        public async Task<IList<CustomerHistoryData>> GetAllHistory(Guid id)
        {
            return CustomerHistory.ToJavaScriptCustomerHistory(await _eventStoreRepository.All(id));
        }


        public void ResolveReceivedQueueToBD(string body)
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






        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
