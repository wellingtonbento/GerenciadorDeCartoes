namespace CardManager.Domain.Repositories.Card
{
    public interface ICardReadRepository
    {
        Task<IList<Entities.Card>> GetCards(long userId);
    }
}
