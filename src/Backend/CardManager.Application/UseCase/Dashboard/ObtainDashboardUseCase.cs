using CardManager.Communication.Responses;
using CardManager.Domain.Identity;
using CardManager.Domain.Repositories.Card;

namespace CardManager.Application.UseCase.Dashboard
{
    public class ObtainDashboardUseCase : IObtainDashboardUseCase
    {
        private readonly ICardReadRepository _cardReadRepository;
        private readonly ILoggedUser _loggedUser;

        public ObtainDashboardUseCase(ICardReadRepository cardReadRepository, ILoggedUser loggedUser)
        {
            _cardReadRepository = cardReadRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseDashboardJson> DashBoard()
        {
            var cards = await _cardReadRepository.GetCards(_loggedUser.GetUserId());

            var totalCreditLimit = cards.Sum(card => card.CreditLimit);
            var totalAvailableCredit = cards.Sum(card => card.AvailableCredit);
            var totalDebit = cards.Sum(card => card.Debit);

            return new ResponseDashboardJson
            {
                CreditLimit = totalCreditLimit,
                AvailableCredit = totalAvailableCredit,
                Debit = totalDebit
            };
        }
    }
}
