namespace CardManager.Exceptions.Exceptions
{
    public class ErrorOnValidationException : CardManagerException
    {
        public IList<string> ErrorMessages { get; set; }

        public ErrorOnValidationException(IList<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }
    }
}
