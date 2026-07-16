using Bogus;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public static class RequestUpdateUserJsonBuilder
    {
        public static RequestUpdateUserJson Build()
        {
            return new Faker<RequestUpdateUserJson>()
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name));
        }
    }
}
