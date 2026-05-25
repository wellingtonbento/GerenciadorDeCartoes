using CardManager.Communication.Requests;
using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.User.Register
{
    public interface IRegisterUserUseCase
    {
        public Task<ResponseUserRegisterJson> ValidateRequest(RequestUserRegisterJson request);
    }
}
