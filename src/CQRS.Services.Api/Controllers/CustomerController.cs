using CQRS.Application.EventSourcedNormalizers;
using CQRS.Application.Interfaces;
using CQRS.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRS.Services.Api.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        private readonly ICustomerAppService _customerAppService;
        //private readonly OcorrenciaProducer _ocorrenciaProducer;

        public CustomerController(ICustomerAppService customerAppService)
            //OcorrenciaProducer ocorrenciaProducer)
        {
            _customerAppService = customerAppService;
            //_ocorrenciaProducer = ocorrenciaProducer;
        }

        [AllowAnonymous]
        [HttpGet("customer-management")]
        public async Task<IEnumerable<CustomerViewModel>> Get()
        {
          //await  _ocorrenciaProducer.SendMessageOcorrencia();
            return await _customerAppService.GetAll();
        }

        [AllowAnonymous]
        [HttpGet("customer-management/{id:guid}")]
        public async Task<CustomerViewModel> Get(Guid id)
        {
            return await _customerAppService.GetById(id);
        }

        [AllowAnonymous]
        //[CustomAuthorize("Customers", "Write")]
        [HttpPost("customer-management")]
        public async Task<IActionResult> Post([FromBody]CustomerViewModel customerViewModel)
        {
            return !ModelState.IsValid ? CustomResponse(ModelState) : CustomResponse(await _customerAppService.Register(customerViewModel));
        }

        //[CustomAuthorize("Customers", "Write")]
        [HttpPut("customer-management")]
        public async Task<IActionResult> Put([FromBody]CustomerViewModel customerViewModel)
        {
            return !ModelState.IsValid ? CustomResponse(ModelState) : CustomResponse(await _customerAppService.Update(customerViewModel));
        }

        //[CustomAuthorize("Customers", "Remove")]
        [HttpDelete("customer-management")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return CustomResponse(await _customerAppService.Remove(id));
        }

        [AllowAnonymous]
        [HttpGet("customer-management/history/{id:guid}")]
        public async Task<IList<CustomerHistoryData>> History(Guid id)
        {
            return await _customerAppService.GetAllHistory(id);
        }
    }
}
