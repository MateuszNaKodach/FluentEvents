using System.Threading.Tasks;

namespace AspNetCoreApiSample.Notifications
{
    public interface IMailService
    {
        Task SendContractTerminatedEmailAsync(int contractId, string reason);
    }
}