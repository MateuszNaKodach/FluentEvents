using System.Threading.Tasks;

namespace AzureSignalRSample.Application
{
    public interface ILightBulbTogglingService
    {
        Task ToggleLightBulbAsync(string notes);
    }
}