using System.Threading.Tasks;

namespace AzureSignalRSample.DomainModel
{
    public interface ILightBulbRepository
    {
        Task<LightBulb> GetLightBulbAsync();
    }
}
