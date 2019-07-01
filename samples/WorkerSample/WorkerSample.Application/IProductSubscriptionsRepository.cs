using System;
using System.Collections.Generic;
using WorkerSample.Domain;

namespace WorkerSample.Application
{
    public interface IProductSubscriptionsRepository
    {
        IEnumerable<ProductSubscription> GetExpiredSubscriptions(DateTime now);
    }
}