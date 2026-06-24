using CardManager.Domain.Repositories;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class UnitOfWorkBuilder
    {
        public static IUnitOfWork Build()
        {
            var mock = new Mock<IUnitOfWork>();

            return mock.Object;
        }
    }
}
