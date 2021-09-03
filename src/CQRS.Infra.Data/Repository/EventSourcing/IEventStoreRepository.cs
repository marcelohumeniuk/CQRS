using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Domain.Core.Events;

namespace CQRS.Infra.Data.Repository.EventSourcing
{
    public interface IEventStoreRepository : IDisposable
    {
        void Store(StoredEvent theEvent);
        Task<IList<StoredEvent>> All(Guid aggregateId);
    }
}