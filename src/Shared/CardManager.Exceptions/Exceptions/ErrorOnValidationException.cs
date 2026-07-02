using System.Net;

namespace CardManager.Exceptions.Exceptions
{
    public class ErrorOnValidationException : CardManagerException
    {
        private readonly IList<string> _erroMessages;

        public ErrorOnValidationException(IList<string> errorMessages) : base(string.Empty)
        {
            _erroMessages = errorMessages;
        }

        public override IList<string> GetErrorMessages() => _erroMessages;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
