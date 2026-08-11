using CardManager.Application.UseCase.Transaction.Obtain;
using CardManager.Application.UseCase.Transaction.Register;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisterTransaction), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RequestRegisterTransaction request, [FromServices] IRegisterTransactionUseCase useCase)
        {
            var result = await useCase.RegisterTransaction(request);

            return Created(string.Empty, result);
        }

        [HttpGet("{cardId}")]
        [ProducesResponseType(typeof(ResponseObtainTransactionsJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] long cardId, [FromServices] IObtainTransactionsUseCase useCase)
        {
            var result = await useCase.ObtainTransactions(cardId);

            return Ok(result);
        }
    }
}
