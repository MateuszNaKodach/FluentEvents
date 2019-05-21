using System.Linq;
using System.Threading.Tasks;
using AzureSignalRSample.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Query
{
    public class LightBulbQueryService : ILightBulbQueryService
    {
        private readonly AppDbContext _appDbContext;

        public LightBulbQueryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> IsLightBulbOnAsync()
        {
            return await _appDbContext.LightBulbs.Select(x => x.IsOn).FirstOrDefaultAsync();
        }
    }
}
