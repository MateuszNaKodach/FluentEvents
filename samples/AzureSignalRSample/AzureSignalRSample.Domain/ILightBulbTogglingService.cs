using System.Threading.Tasks;

namespace AzureSignalRSample.Domain
{
    public interface ILightBulbTogglingService
    {
        Task ToggleLightBulbAsync(string notes);
    }
}