using System.Threading.Tasks;
using AzureSignalRSample.Domain;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Infrastructure
{
    public class LightBulbRepository : ILightBulbRepository
    {
        private readonly LightBulbsDbContext _lightBulbsDbContext;

        public LightBulbRepository(LightBulbsDbContext lightBulbsDbContext)
        {
            _lightBulbsDbContext = lightBulbsDbContext;
        }

        public async Task<LightBulb> GetLightBulbAsync()
        {
            return await _lightBulbsDbContext.LightBulbs.FirstOrDefaultAsync();
        }
    }
}
