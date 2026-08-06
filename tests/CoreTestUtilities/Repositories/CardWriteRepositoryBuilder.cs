using CardManager.Domain.Repositories.Card;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class CardWriteRepositoryBuilder
    {
        public static ICardWriteRepository Build(bool deleteByIdReturn = true)
        {
            var mock = new Mock<ICardWriteRepository>();

            mock.Setup(repository => repository.DeleteById(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(deleteByIdReturn);

            return mock.Object;
        }
    }
}
