using Bogus;
using CardManager.Application.Services.Cryptography;
using CardManager.Domain.Entities;

namespace CoreTestUtilities.Entities
{
    public class UserBuilder
    {
        public static (User user, string password) Build()
        {
            var password = new Faker().Internet.Password();

            var user = new Faker<User>()
                .RuleFor(user => user.Id, () => 1)
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.Password, (f) => PasswordEncripter.EncryptPassword(password));

            return (user, password);
        }
    }
}
