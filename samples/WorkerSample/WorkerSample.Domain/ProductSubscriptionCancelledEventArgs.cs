using System;

namespace WorkerSample.DomainModel
{
    public class ProductSubscriptionCancelledEventArgs
    {
        public DateTime CancellationDateTime { get; set; }
    }
}