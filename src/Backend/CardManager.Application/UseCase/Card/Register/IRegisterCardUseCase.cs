using CardManager.Communication.Requests;
using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Card.Register
{
    public interface IRegisterCardUseCase
    {
        Task<ResponseRegisterCardJson> RegisterCard(RequestCardJson request);
    }
}
