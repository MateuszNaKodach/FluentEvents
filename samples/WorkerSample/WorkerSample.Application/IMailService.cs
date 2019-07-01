using System.Threading.Tasks;

namespace WorkerSample.Application
{
    public interface IMailService
    {
        Task SendEmailAsync(string receiver, string subject, string body);
    }
}
