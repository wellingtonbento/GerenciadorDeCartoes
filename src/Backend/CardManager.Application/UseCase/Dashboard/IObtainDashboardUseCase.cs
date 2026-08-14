using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Dashboard
{
    public interface IObtainDashboardUseCase
    {
        Task<ResponseDashboardJson> DashBoard();
    }
}
