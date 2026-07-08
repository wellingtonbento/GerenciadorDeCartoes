using System.Net;

namespace CardManager.Exceptions.Exceptions
{
    public class IncorrectLoginException : CardManagerException
    {
        public IncorrectLoginException() : base(MessagesException.EMAIL_OR_PASSWORD_INVALID)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
