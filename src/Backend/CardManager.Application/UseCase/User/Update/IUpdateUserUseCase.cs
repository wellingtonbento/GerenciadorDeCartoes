using CardManager.Communication.Requests;

namespace CardManager.Application.UseCase.User.Update
{
    public interface IUpdateUserUseCase
    {
        Task UpdateUser(RequestUpdateUserJson request);
    }
}
