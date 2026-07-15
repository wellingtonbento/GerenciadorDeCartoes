using Bogus;
using CardManager.Domain.Entities;
using CardManager.Domain.Identity;
using Moq;

namespace CoreTestUtilities.Identity
{
    public class LoggedUserBuilder
    {
        public static ILoggedUser Build(User user)
        {
            var mock = new Mock<ILoggedUser>();

            mock.Setup(loggedUser => loggedUser.Get()).ReturnsAsync(user);

            mock.Setup(loggedUser => loggedUser.GetUserId()).Returns(user.Id);

            return mock.Object;
        }
    }
}
