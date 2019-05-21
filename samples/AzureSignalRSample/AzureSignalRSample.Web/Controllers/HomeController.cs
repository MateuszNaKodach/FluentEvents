using System.Diagnostics;
using System.Threading.Tasks;
using AzureSignalRSample.DomainModel;
using AzureSignalRSample.Persistence;
using AzureSignalRSample.Query;
using Microsoft.AspNetCore.Mvc;
using AzureSignalRSample.Web.Models;

namespace AzureSignalRSample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILightBulbTogglingService _lightBulbTogglingService;
        private readonly ILightBulbQueryService _lightBulbQueryService;
        private readonly AppDbContext _appDbContext;

        public HomeController(
            ILightBulbTogglingService lightBulbTogglingService,
            ILightBulbQueryService lightBulbQueryService,
            AppDbContext appDbContext
        )
        {
            _lightBulbTogglingService = lightBulbTogglingService;
            _lightBulbQueryService = lightBulbQueryService;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var lightBulbIsOn = await _lightBulbQueryService.IsLightBulbOnAsync();

            return View(new IndexViewModel {LightBulbIsOn = lightBulbIsOn});
        }

        [HttpPost]
        [Route("api/toggle-light-bulb")]
        public async Task<IActionResult> ToggleLightBulb()
        {
            await _lightBulbTogglingService.ToggleLightBulbAsync("Toggled by API request");
            await _appDbContext.SaveChangesAsync();

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
