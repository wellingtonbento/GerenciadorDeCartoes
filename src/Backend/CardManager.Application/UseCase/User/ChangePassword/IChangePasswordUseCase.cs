using CardManager.Communication.Requests;

namespace CardManager.Application.UseCase.User.ChangePassword
{
    public interface IChangePasswordUseCase
    {
        Task ChangePassword(RequestChangePasswordJson request);
    }
}
