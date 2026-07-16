using CardManager.Application.UseCase.Shared.Validators;
using CardManager.Communication.Requests;
using FluentValidation;

namespace CardManager.Application.UseCase.User.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
    {
        public ChangePasswordValidator()
        {
            RuleFor(request => request.NewPassword).Password();
        }
    }
}
