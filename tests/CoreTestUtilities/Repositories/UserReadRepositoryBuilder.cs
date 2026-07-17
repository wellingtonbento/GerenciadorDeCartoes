using CardManager.Domain.Entities;
using CardManager.Domain.Repositories.User;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public class UserReadRepositoryBuilder
    {
        private readonly Mock<IUserReadRepository> _repository;

        public UserReadRepositoryBuilder() => _repository = new Mock<IUserReadRepository>();

        public void ExistActiveUserWithEmail(string email)
        {
            _repository.Setup(repository => repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
        }
        public void GetEmail(User user)
        {
            _repository.Setup(repository => repository.GetEmail(user.Email)).ReturnsAsync(user);
        }

        public void GetUserWithId(User user)
        {
            _repository.Setup(repository => repository.GetUserWithId(user.Id)).ReturnsAsync(user);
        }

        public IUserReadRepository Build() => _repository.Object;
    }
}
