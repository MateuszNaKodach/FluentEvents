using System;

namespace AspNetCoreApiSample.Domain
{
    public class Contract
    {
        public int Id { get; private set; }
        public string OwnerEmailAddress { get; private set; }
        public bool IsTerminated { get; private set; }
        public string TerminationReason { get; private set; }

        public event EventHandler<ContractTerminatedEventArgs> Terminated;

        public Contract(int id, string ownerEmailAddress)
        {
            Id = id;
            OwnerEmailAddress = ownerEmailAddress;
            IsTerminated = false;
        }

        public void Terminate(string reason)
        {
            if (IsTerminated)
                throw new ContractWasAlreadyTerminatedException();

            TerminationReason = reason ?? throw new ArgumentNullException(nameof(reason));
            IsTerminated = true;

            Terminated?.Invoke(this, new ContractTerminatedEventArgs(reason));
        }
    }
}
