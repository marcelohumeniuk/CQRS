using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Domain.Core;
using CQRS.Domain.Models;
using NetDevPack.Data;

namespace CQRS.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {

        //Read
        Task<Customer> GetById(Guid id);
        Task<Customer> GetByEmail(string email);
        Task<IEnumerable<Customer>> GetAll();



        //Write
        void Add(Customer customer, TypeDB typeDB);
        void Update(Customer customer, TypeDB typeDB);
        void Remove(Customer customer, TypeDB typeDB);
    }
}