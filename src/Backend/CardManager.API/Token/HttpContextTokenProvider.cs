using CardManager.Domain.Security.Tokens;

namespace CardManager.API.Token
{
    sealed class HttpContextTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTokenProvider(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public string GetToken()
        {
            var accessToken = _httpContextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

            var length = "Bearer ".Length;

            return accessToken[length..];
        }
    }
}
