using Api.Test.InlineData;
using CardManager.Communication.Enums;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using CardManager.Infrastructure.DataAccess;
using CoreTestUtilities.Requests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.Transaction.Register
{
    public class RegisterTransactionTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public RegisterTransactionTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success_Debit()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 0, debit: 500);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Debit;
            request.Amount = 100;

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("amount").GetDecimal().Should().Be(request.Amount);
            responseData.RootElement.GetProperty("paymentMethod").GetInt32().Should().Be((int)PaymentMethod.Debit);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var savedTransaction = await dbContext.Transactions.FirstAsync(t => t.CardId == card.Id);

            savedTransaction.PaymentMethod.Should().Be(CardManager.Domain.Entities.PaymentMethod.Debit);
            savedTransaction.Amount.Should().Be(request.Amount);
            savedTransaction.Description.Should().Be(request.Description);

            var updatedCard = await dbContext.Cards.FirstAsync(c => c.Id == card.Id);

            updatedCard.Debit.Should().Be(400);
        }

        [Fact]
        public async Task Success_Credit()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 500);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Credit;
            request.Amount = 100;

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var updatedCard = await dbContext.Cards.FirstAsync(c => c.Id == card.Id);

            updatedCard.CreditBalance.Should().Be(600);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Card_Not_Found(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = 999999;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await AssertSingleError(response, "CARD_NOT_FOUND", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Debit_Insufficient(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 0, debit: 50);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Debit;
            request.Amount = 100;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "TRANSACTION_DEBIT_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Credit_Limit_Exceeded(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 950);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = card.Id;
            request.PaymentMethod = PaymentMethod.Credit;
            request.Amount = 100;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "TRANSACTION_CREDIT_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_CardId_Invalid(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.CardId = 0;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "CARD_ID_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Amount_Invalid(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.Amount = 0;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "TRANSACTION_VALUE_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Description_Empty(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestRegisterTransactionBuilder.Build();
            request.Description = string.Empty;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "DESCRIPTION_REQUIRED", culture);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var request = RequestRegisterTransactionBuilder.Build();

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_TOKEN_REQUIRED));
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_token");

            var request = RequestRegisterTransactionBuilder.Build();

            var response = await _httpClient.PostAsJsonAsync("/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_DENIED));
        }

        private void SetCulture(string culture)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }

        private static async Task AssertSingleError(HttpResponseMessage response, string messageKey, string culture)
        {
            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = MessagesException.ResourceManager.GetString(messageKey, new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }

        private string GenerateToken(CardManager.Domain.Entities.User user)
        {
            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();

            return tokenGenerator.Generate(user);
        }
    }
}
