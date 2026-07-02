using CardManager.Application.Services.AutoMapper;
using CardManager.Application.UseCase.User.Register;
using Microsoft.Extensions.DependencyInjection;

namespace CardManager.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddAutoMapper(services);
            AddUseCases(services);
        }

        private static void AddUseCases(this IServiceCollection services)
        {
           services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        }

        private static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddScoped(options => new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new Mapping());
            }).CreateMapper());
        }
    }
}
