using CardManager.Communication.Requests;

namespace CardManager.Application.UseCase.ChangePassword
{
    public interface IChangePasswordUseCase
    {
        Task ChangePassword(RequestChangePasswordJson request);
    }
}
