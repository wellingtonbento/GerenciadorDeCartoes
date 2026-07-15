using CardManager.Domain.Entities;

namespace CardManager.Domain.Security.Tokens
{
    public interface ITokenGenerator
    {
        string Generate(User user);
    }
}
