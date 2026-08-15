using AutoMapper;
using CardManager.Application.Services.Payment;
using CardManager.Application.Services.Payment.Factory;
using CardManager.Communication.Enums;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Domain.Repositories.Transaction;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Transaction.Register
{
    public class RegisterTransactionUseCase : IRegisterTransactionUseCase
    {
        private readonly ITransactionWriteRepository _writeRepository;
        private readonly ICardReadRepository _cardReadRepository;
        private readonly IPaymentServiceFactory _paymentServiceFactory;
        private readonly ICardUpdateRepository _cardUpdateRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterTransactionUseCase(ITransactionWriteRepository writeRepository, ICardReadRepository cardReadRepository, IPaymentServiceFactory paymentServiceFactory, ICardUpdateRepository cardUpdateRepository, ILoggedUser loggedUser, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _cardReadRepository = cardReadRepository;
            _paymentServiceFactory = paymentServiceFactory;
            _cardUpdateRepository = cardUpdateRepository;
            _loggedUser = loggedUser;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisterTransaction> RegisterTransaction(RequestRegisterTransaction request)
        {
            validateRequest(request);

            var card = await _cardReadRepository.GetCard(request.CardId, _loggedUser.GetUserId());
            if (card is null)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);

            var paymentService = _paymentServiceFactory.GetService(request.PaymentMethod);

            paymentService.Process(card, request.Amount);

            _cardUpdateRepository.Update(card);

            var transaction = _mapper.Map<Domain.Entities.Transaction>(request);

            await _writeRepository.Add(transaction);
            await _unitOfWork.SaveDb();

            return new ResponseRegisterTransaction
            {
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount
            };
        }

        private static void validateRequest(RequestRegisterTransaction request)
        {
            var result = new TransactionValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(erro => erro.ErrorMessage).ToList());
        }
    }
}
