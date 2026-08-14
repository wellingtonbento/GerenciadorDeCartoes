using CardManager.Application.UseCase.Dashboard;
using CardManager.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResponseDashboardJson), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromServices] IObtainDashboardUseCase useCase)
        {
            var result = await useCase.DashBoard();

            return Ok(result);
        }
    }
}
