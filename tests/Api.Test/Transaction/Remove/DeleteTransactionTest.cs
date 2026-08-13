using Api.Test.InlineData;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using CardManager.Infrastructure.DataAccess;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Api.Test.Transaction.Remove
{
    public class DeleteTransactionTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public DeleteTransactionTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success_Debit()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 0, debit: 900);

            var transaction = await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Debit, 100, "Compra no mercado");

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/transactions/{card.Id}/{transaction.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var deletedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);

            deletedTransaction.Should().BeNull();

            var updatedCard = await dbContext.Cards.FirstAsync(c => c.Id == card.Id);

            updatedCard.Debit.Should().Be(1000);
        }

        [Fact]
        public async Task Success_Credit()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 600);

            var transaction = await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Credit, 100, "Restaurante");

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/transactions/{card.Id}/{transaction.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var deletedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);

            deletedTransaction.Should().BeNull();

            var updatedCard = await dbContext.Cards.FirstAsync(c => c.Id == card.Id);

            updatedCard.CreditBalance.Should().Be(500);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Transaction_Not_Found(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            SetCulture(culture);

            var response = await _httpClient.DeleteAsync($"/transactions/{card.Id}/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await AssertSingleError(response, "TRANSACTION_NOT_FOUND", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Transaction_Of_Other_Card(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user);
            var otherCard = await _factory.SeedCardAsync(user);

            var transaction = await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Debit, 100, "Compra");

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            SetCulture(culture);

            var response = await _httpClient.DeleteAsync($"/transactions/{otherCard.Id}/{transaction.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await AssertSingleError(response, "TRANSACTION_NOT_FOUND", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Card_Not_Found(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();
            (var otherUser, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user);

            var transaction = await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Debit, 100, "Compra");

            var token = GenerateToken(otherUser);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            SetCulture(culture);

            var response = await _httpClient.DeleteAsync($"/transactions/{card.Id}/{transaction.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await AssertSingleError(response, "CARD_NOT_FOUND", culture);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.DeleteAsync("/transactions/1/1");

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

            var response = await _httpClient.DeleteAsync("/transactions/1/1");

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
