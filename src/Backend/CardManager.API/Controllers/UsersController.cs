using CardManager.Application.UseCase.User.Profile;
using CardManager.Application.UseCase.User.Register;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserRegisterJson), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromServices] IRegisterUserUseCase useCase, [FromBody] RequestUserRegisterJson request)
        {
            var result = await useCase.ValidateRequest(request);

            return Created(string.Empty, result);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfileUser([FromServices] IGetUserProfileUseCase useCase)
        {
            var result = await useCase.GetUserProfile();

            return Ok(result);
        }
    }
}
