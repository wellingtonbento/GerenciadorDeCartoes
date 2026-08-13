
using CardManager.Application.Services.Payment.Factory;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Domain.Repositories.Transaction;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Transaction.Remove
{
    public class DeleteTransactionUseCase : IDeleteTransactionUseCase
    {
        private readonly ITransactionReadRepository _readRepository;
        private readonly ITransactionDeleteRepository _deleteRepository;
        private readonly ICardReadRepository _cardReadRepository;
        private readonly ICardUpdateRepository _cardUpdateRepository;
        private readonly IPaymentServiceFactory _paymentService;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTransactionUseCase(ITransactionReadRepository readRepository, ITransactionDeleteRepository deleteRepository, ICardReadRepository cardReadRepository, ICardUpdateRepository cardUpdateRepository, IPaymentServiceFactory paymentService, ILoggedUser loggedUser, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _deleteRepository = deleteRepository;
            _cardReadRepository = cardReadRepository;
            _cardUpdateRepository = cardUpdateRepository;
            _paymentService = paymentService;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteTransaction(long cardId, long transactionId)
        {
            var transaction = await _readRepository.ObtainTransaction(transactionId);
            if(transaction is null)
                throw new NotFoundException(MessagesException.TRANSACTION_NOT_FOUND);
            if (transaction.CardId != cardId)
                throw new NotFoundException(MessagesException.TRANSACTION_NOT_FOUND);

            var card = await _cardReadRepository.GetCard(cardId, _loggedUser.GetUserId());
            if(card is null)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);

            var paymentMethod = _paymentService.GetService((Communication.Enums.PaymentMethod)transaction.PaymentMethod);
            paymentMethod.Process(card, -transaction.Amount);

            _cardUpdateRepository.Update(card);
            await _deleteRepository.Delete(transaction.Id);
            await _unitOfWork.SaveDb();
        }
    }
}
