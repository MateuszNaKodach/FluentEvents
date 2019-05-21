using System.Threading.Tasks;
using AzureSignalRSample.DomainModel;
using AzureSignalRSample.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Repositories
{
    public class LightBulbRepository : ILightBulbRepository
    {
        private readonly AppDbContext _appDbContext;

        public LightBulbRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<LightBulb> GetLightBulbAsync()
        {
            return await _appDbContext.LightBulbs.FirstOrDefaultAsync();
        }
    }
}
