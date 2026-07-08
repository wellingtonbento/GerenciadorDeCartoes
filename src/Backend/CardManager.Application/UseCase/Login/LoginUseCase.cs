using CardManager.Application.Services.Cryptography;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using CardManager.Domain.Repositories.User;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Login
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserReadRepository _readRepository;

        public LoginUseCase(IUserReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ResponseUserRegisterJson> Login(RequestLoginJson request)
        {
            var user = await _readRepository.GetEmail(request.Email);
            if (user is null)
                throw new IncorrectLoginException();

            var isPasswordValid = PasswordEncripter.VerifyPassword(request.Password, user!.Password);
            if (!isPasswordValid)
                throw new IncorrectLoginException();


            return new ResponseUserRegisterJson
            {
                Name = user.Name
            };
        }
    }
}
