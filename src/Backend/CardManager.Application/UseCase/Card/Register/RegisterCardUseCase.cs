using AutoMapper;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Card.Register
{
    public class RegisterCardUseCase : IRegisterCardUseCase
    {
        private readonly ICardWriteRepository _repository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegisterCardUseCase(ICardWriteRepository repository, ILoggedUser loggedUser, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseRegisterCardJson> RegisterCard(RequestCardJson request)
        {
            Validate(request);

            var card = _mapper.Map<Domain.Entities.Card>(request);
            card.UserId = _loggedUser.GetUserId();

            await _repository.Add(card);
            await _unitOfWork.SaveDb();

            return new ResponseRegisterCardJson
            {
                Name = card.Name
            };
        }

        private static void Validate(RequestCardJson request)
        {
            var result = new CardValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(erro => erro.ErrorMessage).ToList());
        }
    }
}
