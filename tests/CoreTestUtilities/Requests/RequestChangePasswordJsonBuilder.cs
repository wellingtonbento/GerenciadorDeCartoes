using Bogus;
using CardManager.Communication.Requests;

namespace CoreTestUtilities.Requests
{
    public static class RequestChangePasswordJsonBuilder
    {
        public static RequestChangePasswordJson Build()
        {
            return new Faker<RequestChangePasswordJson>()
                .RuleFor(req => req.CurrentPassword, (f) => f.Internet.Password(8))
                .RuleFor(req => req.NewPassword, (f) => f.Internet.Password(8));
        }
    }
}
