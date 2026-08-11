using AutoMapper;
using CardManager.Communication.Responses;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories.Card;
using CardManager.Domain.Repositories.Transaction;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Transaction.Obtain
{
    public class ObtainTransactionsUseCase : IObtainTransactionsUseCase
    {
        private readonly ITransactionReadRepository _transactionReadRepository;
        private readonly ICardReadRepository _cardReadRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;

        public ObtainTransactionsUseCase(ITransactionReadRepository transactionReadRepository, ICardReadRepository cardReadRepository, ILoggedUser loggedUser, IMapper mapper)
        {
            _transactionReadRepository = transactionReadRepository;
            _cardReadRepository = cardReadRepository;
            _loggedUser = loggedUser;
            _mapper = mapper;
        }

        public async Task<IList<ResponseObtainTransactionsJson>> ObtainTransactions(long cardId)
        {
            var card = await _cardReadRepository.GetCard(cardId, _loggedUser.GetUserId());
            if (card is null)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);

            var transactions = await _transactionReadRepository.ObtainTransactions(card.Id);

            return _mapper.Map<IList<ResponseObtainTransactionsJson>>(transactions);
        }
    }
}
