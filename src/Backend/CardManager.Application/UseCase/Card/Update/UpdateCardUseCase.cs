using AutoMapper;
using CardManager.Communication.Requests;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Card.Update
{
    public class UpdateCardUseCase : IUpdateCardUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly ICardReadRepository _readRepository;
        private readonly ICardUpdateRepository _updateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCardUseCase(ILoggedUser loggedUser, ICardReadRepository readRepository, ICardUpdateRepository updateRepository, IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _readRepository = readRepository;
            _updateRepository = updateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateCard(long cardId, RequestUpdateCardJson request)
        {
            Validate(request);

            var card = await _readRepository.GetCard(cardId, _loggedUser.GetUserId());
            if (card is null)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);
            if (request.CreditLimit < card.CreditBalance)
                throw new ErrorUpdateCardException(MessagesException.CREDIT_LIMIT_LOWER_THAN_THE_CREDIT_SPENT);

            card.Name = request.Name;
            card.CreditLimit = request.CreditLimit;
            card.Debit = request.Debit;

            _updateRepository.Update(card);

            await _unitOfWork.SaveDb();
        }


        private static void Validate(RequestUpdateCardJson request)
        {
            var result = new UpdateCardValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(erro => erro.ErrorMessage).ToList());
        }
    }
}
