using System.Threading.Tasks;
using AzureSignalRSample.Domain;

namespace AzureSignalRSample.Application
{
    public interface ILightBulbRepository
    {
        Task<LightBulb> GetLightBulbAsync();
    }
}
