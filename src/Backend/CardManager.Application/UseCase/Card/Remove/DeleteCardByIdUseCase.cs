
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;

namespace CardManager.Application.UseCase.Card.Remove
{
    public class DeleteCardByIdUseCase : IDeleteCardByIdUseCase
    {
        private readonly ICardWriteRepository _writeRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCardByIdUseCase(ICardWriteRepository writeRepository, ILoggedUser loggedUser, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteCard(long cardId)
        {
            var userId = _loggedUser.GetUserId();
            var deleted = await _writeRepository.DeleteById(cardId, userId);

            if (!deleted)
                throw new NotFoundException(MessagesException.CARD_NOT_FOUND);

            await _unitOfWork.SaveDb();
        }
    }
}
