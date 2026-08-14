using CardManager.Application.Services.AutoMapper;
using CardManager.Application.Services.Payment.Factory;
using CardManager.Application.UseCase.Card.Obtain;
using CardManager.Application.UseCase.Card.Register;
using CardManager.Application.UseCase.Card.Remove;
using CardManager.Application.UseCase.Card.Update;
using CardManager.Application.UseCase.Dashboard;
using CardManager.Application.UseCase.Login;
using CardManager.Application.UseCase.Transaction.ChangeAmount;
using CardManager.Application.UseCase.Transaction.Obtain;
using CardManager.Application.UseCase.Transaction.Register;
using CardManager.Application.UseCase.Transaction.Remove;
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
            services.AddScoped<IObtainCardsUseCase, ObtainCardsUseCase>();
            services.AddScoped<IDeleteCardByIdUseCase, DeleteCardByIdUseCase>();
            services.AddScoped<IUpdateCardUseCase, UpdateCardUseCase>();

            services.AddScoped<IRegisterTransactionUseCase, RegisterTransactionUseCase>();
            services.AddScoped<IObtainTransactionsUseCase, ObtainTransactionsUseCase>();
            services.AddScoped<IChangeAmountUseCase, ChangeAmountUseCase>();
            services.AddScoped<IDeleteTransactionUseCase, DeleteTransactionUseCase>();
            services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();

            services.AddScoped<IObtainDashboardUseCase, ObtainDashboardUseCase>();
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
