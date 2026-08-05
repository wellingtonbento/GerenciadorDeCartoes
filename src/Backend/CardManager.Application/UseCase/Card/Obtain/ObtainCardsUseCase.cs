using AutoMapper;
using CardManager.Communication.Responses;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories.Card;

namespace CardManager.Application.UseCase.Card.Obtain
{
    public class ObtainCardsUseCase : IObtainCardsUseCase
    {
        private readonly ICardReadRepository _repository;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;

        public ObtainCardsUseCase(ICardReadRepository repository, ILoggedUser loggedUser, IMapper mapper)
        {
            _repository = repository;
            _loggedUser = loggedUser;
            _mapper = mapper;
        }

        public async Task<IList<ResponseObtainCardsJson>> GetCards()
        {

            var cards = await _repository.GetCards(_loggedUser.GetUserId());

            return _mapper.Map<IList<ResponseObtainCardsJson>>(cards);
        }
    }
}
