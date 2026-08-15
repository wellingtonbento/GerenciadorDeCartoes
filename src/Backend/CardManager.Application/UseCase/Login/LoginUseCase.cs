using CardManager.Application.Services.Cryptography;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using CardManager.Domain.Repositories.User;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Login
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserReadRepository _readRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginUseCase(IUserReadRepository readRepository, ITokenGenerator tokenGenerator)
        {
            _readRepository = readRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ResponseUserRegisterJson> Login(RequestLoginJson request)
        {
            var user = await _readRepository.GetEmail(request.Email);
            if (user is null)
                throw new IncorrectLoginException();

            var isPasswordValid = PasswordEncripter.VerifyPassword(request.Password, user.Password);
            if (!isPasswordValid)
                throw new IncorrectLoginException();


            return new ResponseUserRegisterJson
            {
                Name = user.Name,
                Tokens = new ResponseTokenJson
                {
                    AccessToken = _tokenGenerator.Generate(user)
                }
            };
        }
    }
}
