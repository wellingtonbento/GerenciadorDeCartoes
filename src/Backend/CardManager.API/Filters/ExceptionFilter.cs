using CardManager.Communication.Responses;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CardManager.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CardManagerException cardManagerException)
                HandleExceptionProject(cardManagerException, context);
            else
                ThrowUnknowException(context);

        }

        private static void HandleExceptionProject(CardManagerException cardManagerException, ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)cardManagerException.GetStatusCode();
            context.Result = new ObjectResult(new ResponseErrorJson(cardManagerException.GetErrorMessages()));
        }

        private static void ThrowUnknowException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(MessagesException.UNKNOWN_ERROR));
        }
    }
}
