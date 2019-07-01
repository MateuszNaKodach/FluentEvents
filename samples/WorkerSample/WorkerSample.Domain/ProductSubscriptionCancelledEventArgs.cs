using System;

namespace WorkerSample.Domain
{
    public class ProductSubscriptionCancelledEventArgs
    {
        public DateTime CancellationDateTime { get; set; }
    }
}