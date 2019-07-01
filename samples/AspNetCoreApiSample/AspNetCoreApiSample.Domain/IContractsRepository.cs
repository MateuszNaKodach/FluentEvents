using System.Threading.Tasks;

namespace AspNetCoreApiSample.DomainModel
{
    public interface IContractsRepository
    {
        Task<Contract> GetContractByIdAsync(int contractId);
    }
}