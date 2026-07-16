using CardManager.Domain.Repositories.User;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class UserUpdateRepositoryBuilder
    {
        public static IUserUpdateRepository Build()
        {
            var mock = new Mock<IUserUpdateRepository>();

            return mock.Object;
        }
    }
}
