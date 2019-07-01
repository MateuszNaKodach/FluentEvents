using System.Threading.Tasks;

namespace AzureSignalRSample.DomainModel
{
    public interface ILightBulbTogglingService
    {
        Task ToggleLightBulbAsync(string notes);
    }
}