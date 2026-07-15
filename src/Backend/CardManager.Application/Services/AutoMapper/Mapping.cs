using AutoMapper;
using CardManager.Communication.Requests;
using CardManager.Communication.Responses;

namespace CardManager.Application.Services.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            RequestToDomain();
            DomainToResponse();
        }

        private void RequestToDomain()
        {
            CreateMap<RequestUserRegisterJson, Domain.Entities.User>()
                .ForMember(destination => destination.Password, option => option.Ignore());
        }

        private void DomainToResponse()
        {
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();
        }
    }
}
