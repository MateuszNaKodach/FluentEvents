using System.Threading.Tasks;

namespace AzureSignalRSample.Application
{
    public class LightBulbTogglingService : ILightBulbTogglingService
    {
        private readonly ILightBulbRepository _lightBulbRepository;

        public LightBulbTogglingService(ILightBulbRepository lightBulbRepository)
        {
            _lightBulbRepository = lightBulbRepository;
        }

        public async Task ToggleLightBulbAsync(string notes)
        {
            var lightBulb = await _lightBulbRepository.GetLightBulbAsync();

            lightBulb?.Toggle(notes);
        }
    }
}
