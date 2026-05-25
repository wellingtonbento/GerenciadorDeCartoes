using AutoMapper;
using CardManager.Communication.Requests;

namespace CardManager.Application.Services.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            RequestToDomain();
        }

        private void RequestToDomain()
        {
            CreateMap<RequestUserRegisterJson, Domain.Entities.User>()
                .ForMember(destination => destination.Password, option => option.Ignore());
        }
    }
}
