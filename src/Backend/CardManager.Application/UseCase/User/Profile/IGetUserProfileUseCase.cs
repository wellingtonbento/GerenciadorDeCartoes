using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.User.Profile
{
    public interface IGetUserProfileUseCase
    {
        Task<ResponseUserProfileJson> GetUserProfile();
    }
}
