using System.Threading.Tasks;

namespace AzureSignalRSample.Query
{
    public interface ILightBulbQueryService
    {
        Task<bool> IsLightBulbOnAsync();
    }
}