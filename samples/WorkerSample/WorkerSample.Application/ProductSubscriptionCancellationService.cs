using System;

namespace WorkerSample.Application
{
    public class ProductSubscriptionCancellationService : IProductSubscriptionCancellationService
    {
        private readonly IProductSubscriptionsRepository _productSubscriptionsRepository;

        public ProductSubscriptionCancellationService(
            IProductSubscriptionsRepository productSubscriptionsRepository
        )
        {
            _productSubscriptionsRepository = productSubscriptionsRepository;
        }

        public void CancelExpiredSubscriptions(DateTime now)
        {
            var expiredSubscriptions = _productSubscriptionsRepository.GetExpiredSubscriptions(now);

            foreach (var expiredSubscription in expiredSubscriptions)
                expiredSubscription.Cancel();
        }
    }
}
