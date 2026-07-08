using CardManager.Application.UseCase.Login;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserRegisterJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromServices] ILoginUseCase useCase, [FromBody] RequestLoginJson request)
        {
            var response = await useCase.Login(request);

            return Ok(response);
        }
    }
}
