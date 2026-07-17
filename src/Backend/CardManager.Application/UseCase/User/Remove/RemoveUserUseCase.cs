using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardManager.Application.UseCase.User.Remove
{
    public class RemoveUserUseCase : IRemoveUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserDeleteRepository _removeRepository;
        private readonly IUserReadRepository _readRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserUseCase(ILoggedUser loggedUser, IUserDeleteRepository removeRepository, IUserReadRepository readRepository, IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _removeRepository = removeRepository;
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task RemoveUser()
        {
            var loggedUser = await _loggedUser.Get();

            var user = await _readRepository.GetUserWithId(loggedUser.Id);

            await _removeRepository.Delete(user!.Id);
            await _unitOfWork.SaveDb();
        }
    }
}
