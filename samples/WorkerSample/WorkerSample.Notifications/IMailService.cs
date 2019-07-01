using System.Threading.Tasks;

namespace WorkerSample.Notifications
{
    public interface IMailService
    {
        Task SendEmailAsync(string receiver, string subject, string body);
    }
}
