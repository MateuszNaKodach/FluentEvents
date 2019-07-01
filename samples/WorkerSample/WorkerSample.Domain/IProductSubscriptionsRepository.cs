using System;
using System.Collections.Generic;

namespace WorkerSample.DomainModel
{
    public interface IProductSubscriptionsRepository
    {
        IEnumerable<ProductSubscription> GetExpiredSubscriptions(DateTime now);
    }
}