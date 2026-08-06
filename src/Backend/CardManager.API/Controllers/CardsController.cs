using CardManager.Application.UseCase.Card.Obtain;
using CardManager.Application.UseCase.Card.Register;
using CardManager.Application.UseCase.Card.Remove;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CardsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisterCardJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromServices] IRegisterCardUseCase useCase, [FromBody] RequestCardJson request)
        {
            var result = await useCase.RegisterCard(request);

            return Created(string.Empty, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<ResponseObtainCardsJson>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromServices] IObtainCardsUseCase useCase)
        {
            var result = await useCase.GetCards();

            return Ok(result);
        }

        [HttpDelete("{cardId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long cardId, [FromServices] IDeleteCardByIdUseCase useCase)
        {
            await useCase.DeleteCard(cardId);

            return NoContent();
        }
    }
}
