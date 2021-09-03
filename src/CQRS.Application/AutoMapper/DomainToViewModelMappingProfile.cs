using AutoMapper;
using CQRS.Application.ViewModels;
using CQRS.Domain.Models;

namespace CQRS.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Customer, CustomerViewModel>();
        }
    }
}
