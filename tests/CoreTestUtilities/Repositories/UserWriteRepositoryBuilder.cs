using CardManager.Domain.Repositories.User;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class UserWriteRepositoryBuilder
    {
        public static IUserWriteRepository Build()
        {
            var mock = new Mock<IUserWriteRepository>();

            return mock.Object;
        }
    }
}
