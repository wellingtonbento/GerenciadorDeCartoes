using CardManager.Domain.Entities;
using CardManager.Domain.Security.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CardManager.Infrastructure.Security.Tokens
{
    public class JwtToken : ITokenGenerator
    {
        private readonly uint _expirationTokenInMunites;
        private readonly string _signingKey;

        public JwtToken(uint expirationTokenInMunites, string signingKey)
        {
            _expirationTokenInMunites = expirationTokenInMunites;
            _signingKey = signingKey;
        }

        public string Generate(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(_expirationTokenInMunites),
                SigningCredentials = new SigningCredentials(Credentials(), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var handler = new JsonWebTokenHandler();

            return handler.CreateToken(tokenDescriptor);
        }

        private SymmetricSecurityKey Credentials()
        {
            var keyBytes = Encoding.UTF8.GetBytes(_signingKey);

            return new SymmetricSecurityKey(keyBytes);
        }
    }
}
