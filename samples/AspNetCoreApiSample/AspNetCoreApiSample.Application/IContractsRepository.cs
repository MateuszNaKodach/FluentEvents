using System.Threading.Tasks;
using AspNetCoreApiSample.Domain;

namespace AspNetCoreApiSample.Application
{
    public interface IContractsRepository
    {
        Task<Contract> GetContractByIdAsync(int contractId);
    }
}