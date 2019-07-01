using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspNetCoreApiSample.Application;
using AspNetCoreApiSample.Domain;
using AspNetCoreApiSample.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApiSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractTerminationService _contractTerminationService;
        private readonly AppDbContext _appDbContext;

        public ContractsController(
            IContractTerminationService contractTerminationService,
            AppDbContext appDbContext
        )
        {
            _contractTerminationService = contractTerminationService;
            _appDbContext = appDbContext;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Terminate(int id = 1, [FromBody] TerminateRequestBody body = null)
        {
            try
            {
                await _contractTerminationService.TerminateContractAsync(id, body?.Reason);
            }
            catch (ContractWasAlreadyTerminatedException)
            {
                return BadRequest("Contract was already terminated");
            }

            await _appDbContext.SaveChangesAsync();

            return Ok("Contract terminated, please see the console for the email log");
        }

        public class TerminateRequestBody
        {
            [Required]
            public string Reason { get; set; }
        }
    }
}
