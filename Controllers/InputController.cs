using System.Threading.Tasks;
using demo_polly.Services;
using Microsoft.AspNetCore.Mvc;

namespace demo_polly.Controllers
{
    [Route("api/demo")]
    public class InputController : Controller
    {
        private INameService _nameService;

        public InputController(INameService nameService)
        {
            _nameService = nameService;
        }

        [Route("timeout/{name}")]
        [HttpGet]
        public IActionResult GetInfoTimeout(string name)
        {
            var info = _nameService.GetNameInfo(name);
            return Ok(info);
        }

        [Route("circuit-breaker/{name}")]
        [HttpGet]
        public async Task<IActionResult> GetInfoCircuitBreaker(string name)
        {
            var info = await _nameService.GetNameInfoAsync(name);
            return Ok(info);
        }
    }
}