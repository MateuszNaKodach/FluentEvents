using System;
using System.Collections.Generic;

namespace WorkerSample.DomainModel
{
    internal interface IProductSubscriptionsRepository
    {
        IEnumerable<ProductSubscription> GetExpiredSubscriptions(DateTime now);
    }
}