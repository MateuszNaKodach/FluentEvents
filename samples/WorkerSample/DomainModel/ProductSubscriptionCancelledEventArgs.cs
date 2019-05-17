using System;

namespace WorkerSample.DomainModel
{
    internal class ProductSubscriptionCancelledEventArgs
    {
        public DateTime CancellationDateTime { get; set; }
    }
}