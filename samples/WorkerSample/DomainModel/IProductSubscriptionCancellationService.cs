using System;

namespace WorkerSample.DomainModel
{
    internal interface IProductSubscriptionCancellationService
    {
        void CancelExpiredSubscriptions(DateTime now);
    }
}