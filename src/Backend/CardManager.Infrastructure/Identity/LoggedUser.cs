using CardManager.Domain.Entities;
using CardManager.Domain.Identity;
using CardManager.Domain.Security.Tokens;
using CardManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CardManager.Infrastructure.Identity
{
    public class LoggedUser : ILoggedUser
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly CardManagerDbContext _context;

        public LoggedUser(ITokenProvider tokenProvider, CardManagerDbContext context)
        {
            _tokenProvider = tokenProvider;
            _context = context;
        }

        public async Task<User> Get()
        {
            var userId = GetUserId();

            return await _context.Users.AsNoTracking().FirstAsync(user => user.Active && user.Id == userId);
        }

        public long GetUserId()
        {
            var accessToken = _tokenProvider.GetToken();

            var handler = new JsonWebTokenHandler();

            var jsonWebToken = handler.ReadJsonWebToken(accessToken);

            var subject = jsonWebToken.Claims.First(claim => claim.Type.Equals(JwtRegisteredClaimNames.Sub));

            return long.Parse(subject.Value);
        }
    }
}
