using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Application.EventSourcedNormalizers;
using CQRS.Application.Interfaces;
using CQRS.Application.ViewModels;
using CQRS.Domain.Commands;
using CQRS.Domain.Interfaces;
using CQRS.Infra.CrossCutting.Bus.ServiceBus;
using CQRS.Infra.Data.Repository.EventSourcing;
using FluentValidation.Results;
using NetDevPack.Mediator;

namespace CQRS.Application.Services
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMediatorHandler _mediator;
        private readonly ServiceBusProducer serviceBusProducer;

        public CustomerAppService(IMapper mapper,
                                  ICustomerRepository customerRepository,
                                  IMediatorHandler mediator,
                                  ServiceBusProducer _serviceBusProducer,
                                  IEventStoreRepository eventStoreRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _mediator = mediator;
            _serviceBusProducer = serviceBusProducer;
            _eventStoreRepository = eventStoreRepository;
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
                var mg = new Funcionario { Nome = "SERVIDOR", Matricula = 12345678, Status = "INATIVO" };

                await serviceBusProducer.SendSBMessage(QueueName.Ocorrencia, mg);
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
