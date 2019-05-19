using System;

namespace WorkerSample.DomainModel
{
    public interface IProductSubscriptionCancellationService
    {
        void CancelExpiredSubscriptions(DateTime now);
    }
}