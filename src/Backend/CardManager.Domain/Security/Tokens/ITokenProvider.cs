namespace CardManager.Domain.Security.Tokens
{
    public interface ITokenProvider
    {
        public string GetToken();
    }
}
