namespace CardManager.Domain.Repositories.Card
{
    public interface ICardWriteRepository
    {
        Task Add(Entities.Card card);
        Task<bool> DeleteById(long cardId, long userId);
    }
}
