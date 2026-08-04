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

namespace Api.Test.Card.Register
{
    public class RegisterCardTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public RegisterCardTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestCardJsonBuilder.Build();

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(request.Name);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var savedCard = await dbContext.Cards.FirstAsync(card => card.Name == request.Name);

            savedCard.UserId.Should().Be(user.Id);
            savedCard.CreditLimit.Should().Be(request.CreditLimit);
            savedCard.CreditBalance.Should().Be(request.CreditBalance);
            savedCard.Debit.Should().Be(request.Debit);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Empty_Name(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestCardJsonBuilder.Build();
            request.Name = string.Empty;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "NAME_REQUIRED", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_CreditLimit_Negative(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestCardJsonBuilder.Build();
            request.CreditLimit = -1;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "CARD_CREDIT_LIMIT_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_CreditBalance_Negative(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestCardJsonBuilder.Build();
            request.CreditBalance = -1;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "CARD_CREDIT_BALANCE_INVALID", culture);
        }

        [Theory]
        [ClassData(typeof(CultureInlineData))]
        public async Task Error_Debit_Negative(string culture)
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestCardJsonBuilder.Build();
            request.Debit = -1;

            SetCulture(culture);

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await AssertSingleError(response, "CARD_DEBIT_BALANCE_INVALID", culture);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var request = RequestCardJsonBuilder.Build();

            var response = await _httpClient.PostAsJsonAsync("/cards", request);

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
