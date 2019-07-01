using System;
using System.Collections.Generic;

namespace WorkerSample.Domain
{
    public interface IProductSubscriptionsRepository
    {
        IEnumerable<ProductSubscription> GetExpiredSubscriptions(DateTime now);
    }
}