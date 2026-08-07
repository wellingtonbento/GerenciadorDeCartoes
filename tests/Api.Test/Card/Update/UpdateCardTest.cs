using Api.Test.InlineData;
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

namespace Api.Test.Card.Update
{
    public class UpdateCardTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public UpdateCardTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 5000, creditBalance: 0);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestUpdateCardJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync($"/cards/{card.Id}", request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var updatedCard = await dbContext.Cards.FirstAsync(c => c.Id == card.Id);

            updatedCard.Name.Should().Be(request.Name);
            updatedCard.CreditLimit.Should().Be(request.CreditLimit);
            updatedCard.Debit.Should().Be(request.Debit);
            updatedCard.CreditBalance.Should().Be(card.CreditBalance);
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestUpdateCardJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/cards/999999", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.CARD_NOT_FOUND));
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_CreditLimit_Lower_Than_Spent(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user, creditLimit: 5000, creditBalance: 1000);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestUpdateCardJsonBuilder.Build();
            request.CreditLimit = 500;

            SetCulture(culture);

            var response = await _httpClient.PutAsJsonAsync($"/cards/{card.Id}", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "CREDIT_LIMIT_LOWER_THAN_THE_CREDIT_SPENT", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Empty_Name(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestUpdateCardJsonBuilder.Build();
            request.Name = string.Empty;

            SetCulture(culture);

            var response = await _httpClient.PutAsJsonAsync($"/cards/{card.Id}", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "NAME_REQUIRED", culture);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var request = RequestUpdateCardJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/cards/1", request);

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

            var request = RequestUpdateCardJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/cards/1", request);

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
