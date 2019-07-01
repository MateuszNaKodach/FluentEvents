using System.Threading.Tasks;
using AzureSignalRSample.Domain;
using AzureSignalRSample.Persistence;
using Microsoft.AspNetCore.SignalR;

namespace AzureSignalRSample.Web.Hubs
{
    public class LightBulbHub : Hub
    {
        private readonly ILightBulbTogglingService _lightBulbTogglingService;
        private readonly AppDbContext _appDbContext;

        public LightBulbHub(ILightBulbTogglingService lightBulbTogglingService, AppDbContext appDbContext)
        {
            _lightBulbTogglingService = lightBulbTogglingService;
            _appDbContext = appDbContext;
        }

        public async Task ToggleLightBulb()
        {
            await _lightBulbTogglingService.ToggleLightBulbAsync("Toggled by SignalR hub method");
            await _appDbContext.SaveChangesAsync();
        }
    }
}
