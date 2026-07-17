using CardManager.Domain.Repositories.User;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class UserDeleteRepositoryBuilder
    {
        public static IUserDeleteRepository Build()
        {
            var mock = new Mock<IUserDeleteRepository>();

            return mock.Object;
        }
    }
}
