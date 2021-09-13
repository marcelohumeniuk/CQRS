using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Domain.Core;
using CQRS.Domain.Interfaces;
using CQRS.Domain.Models;
using CQRS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;

namespace CQRS.Infra.Data.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        protected readonly CQRSContext Db;
        protected readonly CQRSRead_Context DbRead;

        protected readonly DbSet<Customer> DbSet;
        protected readonly DbSet<Customer> DbSetRead;

        public CustomerRepository(CQRSContext context, CQRSRead_Context contextRead)
        {
            //Read
            DbRead = contextRead;
            DbSetRead = DbRead.Set<Customer>();

            //Write
            Db = context;
            DbSet = Db.Set<Customer>();



        }

        public IUnitOfWork UnitOfWork => Db;
        public IUnitOfWork UnitOfWork22 => DbRead;


        //READ
        public async Task<Customer> GetById(Guid id)
        {
            return await DbSetRead.FindAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            //return await DbSetRead.ToListAsync();
            return await DbSet.ToListAsync();
        }

        public async Task<Customer> GetByEmail(string email)
        {
            return await DbSetRead.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
        }


        //WRITE
        public void Add(Customer customer, TypeDB typeDB)
        {
            if (typeDB == TypeDB.StoreRead)
            {
                DbSetRead.Add(customer);
                DbRead.SaveChanges();
            }

            else
                DbSet.Add(customer);
        }

        public void Update(Customer customer, TypeDB typeDB)
        {
            if (typeDB == TypeDB.StoreRead)
            {
                DbSetRead.Update(customer);
                DbRead.SaveChanges();
            }
            else
                DbSet.Update(customer);
        }

        public void Remove(Customer customer, TypeDB typeDB)
        {
            if (typeDB == TypeDB.StoreRead)
            {
                DbSetRead.Remove(customer);
                DbRead.SaveChanges();
            }
            else
                DbSet.Remove(customer);
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
