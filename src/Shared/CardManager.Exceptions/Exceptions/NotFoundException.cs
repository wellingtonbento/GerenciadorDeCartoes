using System.Net;

namespace CardManager.Exceptions.Exceptions
{
    public class NotFoundException : CardManagerException
    {
        public NotFoundException(string message) : base(message) { }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}
