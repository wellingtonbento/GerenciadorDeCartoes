using CardManager.Communication.Requests;
using CardManager.Communication.Responses;

namespace CardManager.Application.UseCase.Transaction.Register
{
    public interface IRegisterTransactionUseCase
    {
        Task<ResponseRegisterTransaction> RegisterTransaction(RequestRegisterTransaction request);
    }
}
