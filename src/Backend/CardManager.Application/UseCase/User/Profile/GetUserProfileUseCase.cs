using AutoMapper;
using CardManager.Communication.Responses;
using CardManager.Domain.Identity;

namespace CardManager.Application.UseCase.User.Profile
{
    public class GetUserProfileUseCase : IGetUserProfileUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;

        public GetUserProfileUseCase(ILoggedUser loggedUser, IMapper mapper)
        {
            _loggedUser = loggedUser;
            _mapper = mapper;
        }

        public async Task<ResponseUserProfileJson> GetUserProfile()
        {
            var loggedUser = await _loggedUser.Get();
            
            return _mapper.Map<ResponseUserProfileJson>(loggedUser);
        }
    }
}
