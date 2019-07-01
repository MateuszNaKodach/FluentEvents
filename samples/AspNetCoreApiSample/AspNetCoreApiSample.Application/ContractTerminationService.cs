using System.Threading.Tasks;

namespace AspNetCoreApiSample.Application
{
    public class ContractTerminationService : IContractTerminationService
    {
        private readonly IContractsRepository _contractsRepository;

        public ContractTerminationService(IContractsRepository contractsRepository)
        {
            _contractsRepository = contractsRepository;
        }

        public async Task TerminateContractAsync(int contractId, string reason)
        {
            var contract = await _contractsRepository.GetContractByIdAsync(contractId);

            contract.Terminate(reason);
        }
    }
}
