using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using CardManager.Infrastructure.DataAccess;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Api.Test.Card.Remove
{
    public class DeleteCardTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public DeleteCardTest(CustomWebApplicationFactory factory)
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

            var response = await _httpClient.DeleteAsync($"/cards/{card.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            var deletedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == card.Id);

            deletedCard.Should().BeNull();
        }

        [Fact]
        public async Task Error_Card_Not_Found()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var token = GenerateToken(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync("/cards/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.CARD_NOT_FOUND));
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.DeleteAsync("/cards/1");

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

            var response = await _httpClient.DeleteAsync("/cards/1");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_DENIED));
        }

        private string GenerateToken(CardManager.Domain.Entities.User user)
        {
            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();

            return tokenGenerator.Generate(user);
        }
    }
}
