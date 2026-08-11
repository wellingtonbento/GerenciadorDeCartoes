using Api.Test.InlineData;
using CardManager.Communication.Responses;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.Transaction.Obtain
{
    public class ObtainTransactionsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public ObtainTransactionsTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();
            var card = await _factory.SeedCardAsync(user);

            await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Debit, 100, "Compra no mercado");
            await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Credit, 200, "Restaurante");

            var token = GenerateToken(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/transactions/{card.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var transactions = await response.Content.ReadFromJsonAsync<IList<ResponseObtainTransactionsJson>>();

            transactions.Should().NotBeNull();
            transactions!.Should().HaveCount(2);
            transactions.Should().Contain(t => t.Amount == 100 && t.Description == "Compra no mercado");
            transactions.Should().Contain(t => t.Amount == 200 && t.Description == "Restaurante");
        }

        [Fact]
        public async Task Success_Only_Transactions_Of_The_Card()
        {
            (var user, _) = await _factory.SeedUserAsync();
            var card = await _factory.SeedCardAsync(user);
            var otherCard = await _factory.SeedCardAsync(user);

            await _factory.SeedTransactionAsync(card, CardManager.Domain.Entities.PaymentMethod.Debit, 100, "Cartão principal");
            await _factory.SeedTransactionAsync(otherCard, CardManager.Domain.Entities.PaymentMethod.Credit, 500, "Outro cartão");

            var token = GenerateToken(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/transactions/{card.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var transactions = await response.Content.ReadFromJsonAsync<IList<ResponseObtainTransactionsJson>>();

            transactions.Should().HaveCount(1);
            transactions![0].Amount.Should().Be(100);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Card_Not_Found(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            SetCulture(culture);

            var response = await _httpClient.GetAsync("/transactions/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await AssertSingleError(response, "CARD_NOT_FOUND", culture);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.GetAsync("/transactions/1");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_TOKEN_REQUIRED));
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
