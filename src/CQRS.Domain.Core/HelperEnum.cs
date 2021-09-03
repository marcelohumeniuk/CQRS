using System;

namespace CQRS.Domain.Core
{
    [Flags]
    public enum TypeDB: ushort
    {
        StorePrincial = 0,  // 0
        StoreRead = 1,  // 1

    }
}
