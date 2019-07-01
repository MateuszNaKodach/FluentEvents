using System.Threading.Tasks;

namespace AspNetCoreApiSample.Application
{
    public interface IContractTerminationService
    {
        Task TerminateContractAsync(int contractId, string reason);
    }
}