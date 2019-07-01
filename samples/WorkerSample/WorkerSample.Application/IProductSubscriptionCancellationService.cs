using System;

namespace WorkerSample.Application
{
    public interface IProductSubscriptionCancellationService
    {
        void CancelExpiredSubscriptions(DateTime now);
    }
}