using CardManager.Application.Services.Payment;
using CardManager.Application.Services.Payment.Factory;
using CardManager.Communication.Enums;
using CardManager.Exceptions;
using CardManager.Exceptions.Exceptions;
using CoreTestUtilities.Entities;
using FluentAssertions;

namespace UseCases.Test.Transaction.Register
{
    public class PaymentServiceTests
    {
        [Fact]
        public void Debit_Success()
        {
            var card = CardBuilder.Build();
            card.Debit = 1000;

            var service = new DebitPaymentService();

            service.Process(card, 100);

            card.Debit.Should().Be(900);
        }

        [Fact]
        public void Debit_Insufficient_Error()
        {
            var card = CardBuilder.Build();
            card.Debit = 50;

            var service = new DebitPaymentService();

            Action act = () => service.Process(card, 100);

            act.Should().Throw<ErrorOnValidationException>()
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_DEBIT_INVALID));
        }

        [Fact]
        public void Credit_Success()
        {
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 500;

            var service = new CreditPaymentService();

            service.Process(card, 100);

            card.CreditBalance.Should().Be(600);
        }

        [Fact]
        public void Credit_Exceeds_Limit_Error()
        {
            var card = CardBuilder.Build();
            card.CreditLimit = 1000;
            card.CreditBalance = 950;

            var service = new CreditPaymentService();

            Action act = () => service.Process(card, 100);

            act.Should().Throw<ErrorOnValidationException>()
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.TRANSACTION_CREDIT_INVALID));
        }

        [Fact]
        public void Factory_Returns_Debit()
        {
            var factory = new PaymentServiceFactory();

            factory.GetService(PaymentMethod.Debit).Should().BeOfType<DebitPaymentService>();
        }

        [Fact]
        public void Factory_Returns_Credit()
        {
            var factory = new PaymentServiceFactory();

            factory.GetService(PaymentMethod.Credit).Should().BeOfType<CreditPaymentService>();
        }

        [Fact]
        public void Factory_Invalid_Method_Error()
        {
            var factory = new PaymentServiceFactory();

            Action act = () => factory.GetService((PaymentMethod)99);

            act.Should().Throw<ErrorOnValidationException>()
                .Where(error => error.GetErrorMessages().Count == 1 && error.GetErrorMessages().Contains(MessagesException.PAYMENT_METHOD_INVALID));
        }
    }
}
