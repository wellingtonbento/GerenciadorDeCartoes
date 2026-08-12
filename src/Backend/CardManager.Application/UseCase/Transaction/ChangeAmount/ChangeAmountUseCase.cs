using CardManager.Application.Services.Payment.Factory;
using CardManager.Communication.Requests;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Domain.Repositories.Transaction;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Transaction.ChangeAmount
{
    public class ChangeAmountUseCase : IChangeAmountUseCase
    {
        private readonly ITransactionReadRepository _readRepository;
        private readonly ITransactionUpdateRepository _updateRepository;
        private readonly ICardReadRepository _cardReadRepository;
        private readonly ICardUpdateRepository _cardUpdateRepository;
        private readonly IPaymentServiceFactory _paymentService;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeAmountUseCase(ITransactionReadRepository readRepository, ITransactionUpdateRepository updateRepository, ICardReadRepository cardReadRepository, ICardUpdateRepository cardUpdateRepository, IPaymentServiceFactory paymentService, ILoggedUser loggedUser, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _updateRepository = updateRepository;
            _cardReadRepository = cardReadRepository;
            _cardUpdateRepository = cardUpdateRepository;
            _paymentService = paymentService;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task ChangeAmount(long transactionId, long cardId, RequestChangeAmountJson request)
        {
            Validator(request);

            var transaction = await _readRepository.ObtainTransaction(transactionId);
            if (transaction is null)
                throw new NotFoundException(MessagesException.TRANSACTION_NOT_FOUND);
            if (transaction.CardId != cardId)
                throw new NotFoundException(MessagesException.TRANSACTION_NOT_FOUND);

            var card = await _cardReadRepository.GetCard(cardId, _loggedUser.GetUserId());
            if (card is null)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);

            var paymentService = _paymentService.GetService((CardManager.Communication.Enums.PaymentMethod)transaction.PaymentMethod);

            paymentService.Process(card, -transaction.Amount);   
            paymentService.Process(card, request.Amount);        

            _cardUpdateRepository.Update(card);
            await _updateRepository.UpdateAmount(transactionId, request.Amount);
            await _unitOfWork.SaveDb();
        }

        private static void Validator(RequestChangeAmountJson request)
        {
            var result = new ChangeAmountValidator().Validate(request);

            if (result.IsValid == false)
                throw new ErrorOnValidationException(result.Errors.Select(erro => erro.ErrorMessage).ToList());
        }
    }
}
