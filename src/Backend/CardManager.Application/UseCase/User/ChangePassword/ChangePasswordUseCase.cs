using CardManager.Application.Services.Cryptography;
using CardManager.Communication.Requests;
using CardManager.Domain.Identity;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using FluentValidation.Results;

namespace CardManager.Application.UseCase.User.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly ILoggedUser _loggedUser;

        public ChangePasswordUseCase(ILoggedUser loggedUser) => _loggedUser = loggedUser;

        public async Task ChangePassword(RequestChangePasswordJson request)
        {
            var loggedUser = await _loggedUser.Get();

            Validate(request, loggedUser);
        }

        private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
        {
            var result = new ChangePasswordValidator().Validate(request);

            if (PasswordEncripter.VerifyPassword(request.CurrentPassword, loggedUser.Password) == false)
                result.Errors.Add(new ValidationFailure(string.Empty, MessagesException.VALIDATION_CURRENT_PASSWORD));

            if (result.IsValid == false)
                throw new ErrorOnValidationException(result.Errors.Select(erro => erro.ErrorMessage).ToList());
        }
    }
}
