using System.Net;

namespace CardManager.Exceptions.Exceptions
{
    public abstract class CardManagerException : SystemException
    {
        protected CardManagerException(string message) : base(message) { }

        public abstract IList<string> GetErrorMessages();
        public abstract HttpStatusCode GetStatusCode();
    }
}
