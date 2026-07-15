using Bogus;
using CardManager.Domain.Entities;
using CardManager.Domain.Security.Tokens;
using Moq;

namespace CoreTestUtilities.Repositories
{
    public static class TokenGeneratorBuilder
    {
        public static ITokenGenerator Build()
        {
            var mock = new Mock<ITokenGenerator>();

            var fakeToken = new Faker().Random.String2(32, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");

            mock.Setup(generator => generator.Generate(It.IsAny<User>())).Returns(fakeToken);

            return mock.Object;
        }
    }
}
