using System.Threading.Tasks;
using AzureSignalRSample.Application;
using Microsoft.AspNetCore.SignalR;

namespace AzureSignalRSample.Web.Hubs
{
    public class LightBulbHub : Hub
    {
        private readonly ILightBulbTogglingService _lightBulbTogglingService;

        public LightBulbHub(ILightBulbTogglingService lightBulbTogglingService)
        {
            _lightBulbTogglingService = lightBulbTogglingService;
        }

        public async Task ToggleLightBulb()
        {
            await _lightBulbTogglingService.ToggleLightBulbAsync("Toggled by SignalR hub method");
        }
    }
}
