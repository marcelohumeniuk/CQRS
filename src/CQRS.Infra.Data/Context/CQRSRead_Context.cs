using CQRS.Domain.Models;
using CQRS.Infra.Data.Mappings;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using NetDevPack.Messaging;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Infra.Data.Context
{
    public sealed class CQRSRead_Context : DbContext, IUnitOfWork
    {

        public CQRSRead_Context(DbContextOptions<CQRSRead_Context> options) : base(options)
        {
          
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Customer> Customers { get; set; }

        public async Task<bool> Commit()
        {
            var success = await SaveChangesAsync() > 0;

            return success;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfiguration(new CustomerMap());
                        
            base.OnModelCreating(modelBuilder);
        }

     
    }

}
