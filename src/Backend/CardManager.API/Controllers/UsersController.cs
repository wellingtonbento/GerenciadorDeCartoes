using CardManager.Application.UseCase.User.ChangePassword;
using CardManager.Application.UseCase.User.Profile;
using CardManager.Application.UseCase.User.Register;
using CardManager.Application.UseCase.User.Remove;
using CardManager.Application.UseCase.User.Update;
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
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
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

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromServices] IUpdateUserUseCase useCase,
            [FromBody] RequestUpdateUserJson request)
        {
            await useCase.UpdateUser(request);

            return NoContent();
        }

        [HttpPatch("password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromServices] IChangePasswordUseCase useCase,
            [FromBody] RequestChangePasswordJson request)
        {
            await useCase.ChangePassword(request);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Remove([FromServices] IRemoveUserUseCase useCase)
        {
            await useCase.RemoveUser();

            return NoContent();
        }
    }
}
