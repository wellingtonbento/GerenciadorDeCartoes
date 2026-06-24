using AutoMapper;
using CardManager.Application.Services.AutoMapper;

namespace CoreTestUtilities.Mapper
{
    public static class MapperBuilder
    {
        public static IMapper Build()
        {
            return new MapperConfiguration(config =>
            {
                config.AddProfile(new Mapping());
            }).CreateMapper();
        }
    }
}
