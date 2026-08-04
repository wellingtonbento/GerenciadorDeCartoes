using CardManager.Application.Services.AutoMapper;
using CardManager.Application.UseCase.Card.Register;
using CardManager.Application.UseCase.Login;
using CardManager.Application.UseCase.User.ChangePassword;
using CardManager.Application.UseCase.User.Profile;
using CardManager.Application.UseCase.User.Register;
using CardManager.Application.UseCase.User.Remove;
using CardManager.Application.UseCase.User.Update;
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
            services.AddScoped<ILoginUseCase, LoginUseCase>();

            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IRemoveUserUseCase, RemoveUserUseCase>();

            services.AddScoped<IRegisterCardUseCase, RegisterCardUseCase>();
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
