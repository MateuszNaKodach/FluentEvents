using System;

namespace WorkerSample.Domain
{
    public interface IProductSubscriptionCancellationService
    {
        void CancelExpiredSubscriptions(DateTime now);
    }
}