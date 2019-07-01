namespace AspNetCoreApiSample.Domain
{
    public class ContractTerminatedEventArgs
    {
        public string Reason { get; }

        public ContractTerminatedEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}