using CardManager.Communication.Requests;
using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Login
{
    public interface ILoginUseCase
    {
        public Task<ResponseUserRegisterJson> Login(RequestLoginJson request);
    }
}
