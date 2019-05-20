using System.Threading.Tasks;

namespace AspNetCoreApiSample.DomainModel
{
    public interface IContractTerminationService
    {
        Task TerminateContractAsync(int contractId, string reason);
    }
}