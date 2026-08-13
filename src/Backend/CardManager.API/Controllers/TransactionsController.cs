using CardManager.Application.UseCase.Transaction.ChangeAmount;
using CardManager.Application.UseCase.Transaction.Obtain;
using CardManager.Application.UseCase.Transaction.Register;
using CardManager.Application.UseCase.Transaction.Remove;
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

        [HttpPatch("{cardId}/{transactionId}/amount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeAmount([FromRoute] long cardId, [FromRoute] long transactionId,
            [FromBody] RequestChangeAmountJson request, [FromServices] IChangeAmountUseCase useCase)
        {
            await useCase.ChangeAmount(transactionId, cardId, request);
            return NoContent();
        }

        [HttpDelete("{cardId}/{transactionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long cardId, [FromRoute] long transactionId, [FromServices] IDeleteTransactionUseCase useCase)
        {
            await useCase.DeleteTransaction(cardId, transactionId);
            return NoContent();
        }
    }
}
