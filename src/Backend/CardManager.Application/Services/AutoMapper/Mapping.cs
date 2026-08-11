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

            CreateMap<RequestCardJson, Domain.Entities.Card>();

            CreateMap<RequestRegisterTransaction, Domain.Entities.Transaction>();
        }

        private void DomainToResponse()
        {
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();

            CreateMap<Domain.Entities.Card, ResponseObtainCardsJson>()
                 .ForMember(destination => destination.AvailableCredit,
               option => option.MapFrom(source => source.AvailableCredit));

            CreateMap<Domain.Entities.Transaction, ResponseObtainTransactionsJson>()
                .ForMember(destination => destination.CreatedOn,
                option => option.MapFrom(source => source.CreatedOn.Date));
        }
    }
}
