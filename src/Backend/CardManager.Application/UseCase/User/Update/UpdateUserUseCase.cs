using CardManager.Communication.Requests;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.User;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.User.Update
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUserUpdateRepository _userUpdateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserUseCase(ILoggedUser loggedUser, IUserReadRepository userReadRepository, IUserUpdateRepository userUpdateRepository, IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _userReadRepository = userReadRepository;
            _userUpdateRepository = userUpdateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateUser(RequestUpdateUserJson request)
        {
            var loggedUser = await _loggedUser.Get();

            await Validate(request, loggedUser);

            loggedUser.Name = request.Name;
            loggedUser.Email = request.Email;

            _userUpdateRepository.UpdateProfile(loggedUser);

            await _unitOfWork.SaveDb();
        }

        private async Task Validate(RequestUpdateUserJson request, Domain.Entities.User loggedUser)
        {
            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            if(loggedUser.Email.Equals(request.Email) == false)
            {
                var userExist = await _userReadRepository.ExistActiveUserWithEmail(request.Email);
                if(userExist)
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure("email", MessagesException.EMAIL_REQUIRED));
            }

            if(result.IsValid  == false)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
