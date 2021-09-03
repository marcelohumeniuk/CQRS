using AutoMapper;
using CQRS.Application.ViewModels;
using CQRS.Domain.Commands;
using CQRS.Domain.Models;

namespace CQRS.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CustomerViewModel, RegisterNewCustomerCommand>()
                .ConstructUsing(c => new RegisterNewCustomerCommand(c.Name, c.Email, c.BirthDate));
            CreateMap<CustomerViewModel, UpdateCustomerCommand>()
                .ConstructUsing(c => new UpdateCustomerCommand(c.Id, c.Name, c.Email, c.BirthDate));



            CreateMap<CustomerViewModel, Customer>();
               
        }
    }
}
