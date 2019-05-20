using System;
using AsyncEvent;

namespace AspNetCoreApiSample.Events
{
    public class ContractEvents
    {
        public int Id { get; set; }

#pragma warning disable 67 // Used for projection
        public event AsyncEventHandler<ContractTerminatedEventArgs> Terminated;
#pragma warning restore 67

        public class ContractTerminatedEventArgs
        {
            public string Reason { get; set; }
        }
    }
}