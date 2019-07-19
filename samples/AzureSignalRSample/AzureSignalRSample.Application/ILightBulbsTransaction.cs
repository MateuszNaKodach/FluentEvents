using System.Threading.Tasks;

namespace AzureSignalRSample.Application
{
    public interface ILightBulbsTransaction
    {
        Task CommitAsync();
    }
}
