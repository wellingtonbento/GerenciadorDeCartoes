using CardManager.Application.Services.Cryptography;
using CardManager.Communication.Requests;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories.User;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using FluentValidation.Results;

namespace CardManager.Application.UseCase.User.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateRepository _userUpdateRepository;

        public ChangePasswordUseCase(ILoggedUser loggedUser, IUserUpdateRepository userUpdateRepository)
        {
            _loggedUser = loggedUser;
            _userUpdateRepository = userUpdateRepository;
        }

        public async Task ChangePassword(RequestChangePasswordJson request)
        {
            var loggedUser = await _loggedUser.Get();

            Validate(request, loggedUser);

            var hashedPassword = PasswordEncripter.EncryptPassword(request.NewPassword);

            await _userUpdateRepository.UpdatePassword(loggedUser.Id, hashedPassword);
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
