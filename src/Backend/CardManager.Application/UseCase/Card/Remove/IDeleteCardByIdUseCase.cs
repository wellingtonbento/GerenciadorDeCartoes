namespace CardManager.Application.UseCase.Card.Remove
{
    public interface IDeleteCardByIdUseCase
    {
        Task DeleteCard(long cardId);
    }
}
