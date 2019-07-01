using System.Threading.Tasks;
using AspNetCoreApiSample.Application;
using AspNetCoreApiSample.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreApiSample.Repositories
{
    public class ContractsRepository : IContractsRepository
    {
        private readonly AppDbContext _appDbContext;

        public ContractsRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Contract> GetContractByIdAsync(int contractId)
        {
            return await _appDbContext.Contracts.FirstOrDefaultAsync(x => x.Id == contractId);
        }
    }
}
