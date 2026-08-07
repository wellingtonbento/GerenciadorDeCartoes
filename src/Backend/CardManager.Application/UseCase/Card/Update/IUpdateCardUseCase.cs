using CardManager.Communication.Requests;

namespace CardManager.Application.UseCase.Card.Update
{
    public interface IUpdateCardUseCase
    {
        Task UpdateCard(long cardId, RequestUpdateCardJson request);
    }
}
