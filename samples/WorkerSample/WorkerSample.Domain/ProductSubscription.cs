using System;

namespace WorkerSample.Domain
{
    public class ProductSubscription
    {
        public int Id { get; private set; }
        public string CustomerEmailAddress { get; private set; }
        public DateTime ExpirationDateTime { get; private set; }
        public ProductSubscriptionStatus Status { get; private set; }

        public event EventHandler<ProductSubscriptionCancelledEventArgs> Cancelled;

        public ProductSubscription(int id, string customerEmailAddress, DateTime expirationDateTime)
        {
            Id = id;
            CustomerEmailAddress = customerEmailAddress;
            ExpirationDateTime = expirationDateTime;
            Status = ProductSubscriptionStatus.ActivationPending;
        }

        public void Cancel()
        {
            if (Status == ProductSubscriptionStatus.Cancelled)
                throw new ProductSubscriptionWasAlreadyCancelledException();

            Status = ProductSubscriptionStatus.Cancelled;

            Cancelled?.Invoke(this, new ProductSubscriptionCancelledEventArgs());
        }
    }
}
