using CardManager.Communication.Requests;
using CardManager.Exceptions;
using FluentValidation;

namespace CardManager.Application.UseCase.User.Update
{
    public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
    {
        public UpdateUserValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage(MessagesException.NAME_REQUIRED);

            RuleFor(request => request.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(MessagesException.EMAIL_REQUIRED)
                .EmailAddress()
                .WithMessage(MessagesException.EMAIL_INVALID);
        }
    }
}
