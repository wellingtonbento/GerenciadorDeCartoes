using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Card.Obtain
{
    public interface IObtainCardsUseCase
    {
        Task<IList<ResponseObtainCardsJson>> GetCards();
    }
}
