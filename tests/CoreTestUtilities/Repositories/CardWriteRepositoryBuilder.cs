using CardManager.Domain.Repositories.Card;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class CardWriteRepositoryBuilder
    {
        public static ICardWriteRepository Build()
        {
            var mock = new Mock<ICardWriteRepository>();

            return mock.Object;
        }
    }
}
