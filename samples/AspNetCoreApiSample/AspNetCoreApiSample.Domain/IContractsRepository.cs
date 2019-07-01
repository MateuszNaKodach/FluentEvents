using System.Threading.Tasks;

namespace AspNetCoreApiSample.Domain
{
    public interface IContractsRepository
    {
        Task<Contract> GetContractByIdAsync(int contractId);
    }
}