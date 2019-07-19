using System.Threading.Tasks;
using AzureSignalRSample.Domain;

namespace AzureSignalRSample.Application
{
    public class LightBulbTogglingService : ILightBulbTogglingService
    {
        private readonly ILightBulbRepository _lightBulbRepository;
        private readonly ILightBulbsTransaction _lightBulbsTransaction;

        public LightBulbTogglingService(ILightBulbRepository lightBulbRepository, ILightBulbsTransaction lightBulbsTransaction)
        {
            _lightBulbRepository = lightBulbRepository;
            _lightBulbsTransaction = lightBulbsTransaction;
        }

        public async Task ToggleLightBulbAsync(string notes)
        {
            var lightBulb = await _lightBulbRepository.GetLightBulbAsync();

            lightBulb?.Toggle(notes);

            await _lightBulbsTransaction.CommitAsync();
        }
    }
}
