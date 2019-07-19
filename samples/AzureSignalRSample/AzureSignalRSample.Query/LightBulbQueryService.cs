using System.Linq;
using System.Threading.Tasks;
using AzureSignalRSample.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Query
{
    public class LightBulbQueryService : ILightBulbQueryService
    {
        private readonly LightBulbsDbContext _lightBulbsDbContext;

        public LightBulbQueryService(LightBulbsDbContext lightBulbsDbContext)
        {
            _lightBulbsDbContext = lightBulbsDbContext;
        }

        public async Task<bool> IsLightBulbOnAsync()
        {
            return await _lightBulbsDbContext.LightBulbs.Select(x => x.IsOn).FirstOrDefaultAsync();
        }
    }
}
