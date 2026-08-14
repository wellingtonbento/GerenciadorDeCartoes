using CardManager.Communication.Responses;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.Dashboard
{
    public class DashboardTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public DashboardTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var firstCard = await _factory.SeedCardAsync(user, creditLimit: 1000, creditBalance: 200, debit: 300);
            var secondCard = await _factory.SeedCardAsync(user, creditLimit: 2000, creditBalance: 500, debit: 100);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/dashboard");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dashboard = await response.Content.ReadFromJsonAsync<ResponseDashboardJson>();

            dashboard.Should().NotBeNull();
            dashboard!.CreditLimit.Should().Be(firstCard.CreditLimit + secondCard.CreditLimit);
            dashboard.AvailableCredit.Should().Be((firstCard.CreditLimit - firstCard.CreditBalance) + (secondCard.CreditLimit - secondCard.CreditBalance));
            dashboard.Debit.Should().Be(firstCard.Debit + secondCard.Debit);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.GetAsync("/dashboard");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_TOKEN_REQUIRED));
        }

        private string GenerateToken(CardManager.Domain.Entities.User user)
        {
            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();

            return tokenGenerator.Generate(user);
        }
    }
}
