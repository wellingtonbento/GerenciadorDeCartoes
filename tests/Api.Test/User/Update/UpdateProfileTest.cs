using CardManager.Domain.Security.Tokens;
using CoreTestUtilities.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.User.Update
{
    public class UpdateProfileTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public UpdateProfileTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();
            var token = tokenGenerator.Generate(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = RequestUpdateUserJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var request = RequestUpdateUserJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(CardManager.Exceptions.MessagesException.ACCESS_TOKEN_REQUIRED));
        }

        [Fact]
        public async Task Error_Token_Invalid()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_token");

            var request = RequestUpdateUserJsonBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(CardManager.Exceptions.MessagesException.ACCESS_DENIED));
        }

        [Fact]
        public async Task Error_Invalid_Request()
        {
            (var user, _) = await _factory.SeedUserAsync();

            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();
            var token = tokenGenerator.Generate(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidRequest = new { Name = string.Empty, Email = "invalid" };

            var response = await _httpClient.PutAsJsonAsync("/users", invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().NotBeEmpty();
        }
    }
}
