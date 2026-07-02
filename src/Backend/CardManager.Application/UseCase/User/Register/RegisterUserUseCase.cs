using AutoMapper;
using CardManager.Application.Services.Cryptography;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.User;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using FluentValidation.Results;

namespace CardManager.Application.UseCase.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserWriteRepository _writeRepository;
        private readonly IUserReadRepository _readRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public RegisterUserUseCase(IUserWriteRepository writeRepository, IUserReadRepository readRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseUserRegisterJson> ValidateRequest(RequestUserRegisterJson request)
        {
            await Validate(request);

            var user = _mapper.Map<Domain.Entities.User>(request);
            user.Password = PasswordEncripter.EncryptPassword(request.Password);

            await _writeRepository.Add(user);

            await _unitOfWork.SaveDb();

            return new ResponseUserRegisterJson
            {
                Name = user.Name
            };
        }

        private async Task Validate(RequestUserRegisterJson request)
        {
            var validator = new RegisterUserValidator();

            var result = validator.Validate(request);

            var emailExist = await _readRepository.ExistActiveUserWithEmail(request.Email);
            if (emailExist)
                result.Errors.Add(new ValidationFailure(string.Empty, MessagesException.EMAIL_ALREADY_REGISTERED));

            if (!result.IsValid)
            {
                var errorMessage = result.Errors.Select(erro => erro.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessage);
            }
        }
    }
}
