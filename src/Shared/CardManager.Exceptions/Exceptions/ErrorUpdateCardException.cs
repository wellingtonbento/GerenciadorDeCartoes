using System.Net;

namespace CardManager.Exceptions.Exceptions
{
    public class ErrorUpdateCardException : CardManagerException
    {
        private readonly string Message;

        public ErrorUpdateCardException(string message) : base(string.Empty)
        {
            Message = message;
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
