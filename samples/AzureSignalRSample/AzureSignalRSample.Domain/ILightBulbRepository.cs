using System.Threading.Tasks;

namespace AzureSignalRSample.Domain
{
    public interface ILightBulbRepository
    {
        Task<LightBulb> GetLightBulbAsync();
    }
}
