using System.Threading.Tasks;

namespace AspNetCoreApiSample.Domain
{
    public interface IContractTerminationService
    {
        Task TerminateContractAsync(int contractId, string reason);
    }
}