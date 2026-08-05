using CardManager.Communication.Responses;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.Card.Obtain
{
    public class ObtainCardsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public ObtainCardsTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var card = await _factory.SeedCardAsync(user);

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/cards");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var cards = await response.Content.ReadFromJsonAsync<IList<ResponseObtainCardsJson>>();

            cards.Should().NotBeNull();
            cards!.Should().ContainSingle();

            cards[0].Name.Should().Be(card.Name);
            cards[0].CreditLimit.Should().Be(card.CreditLimit);
            cards[0].Debit.Should().Be(card.Debit);
            cards[0].AvailableCredit.Should().Be(card.CreditLimit - card.CreditBalance);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.GetAsync("/cards");

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
