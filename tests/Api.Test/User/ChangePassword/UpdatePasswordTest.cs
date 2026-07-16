using CardManager.Domain.Security.Tokens;
using CoreTestUtilities.Entities;
using CoreTestUtilities.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.User.ChangePassword
{
    public class UpdatePasswordTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public UpdatePasswordTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Success()
        {
            (var user, var password) = await _factory.SeedUserAsync();

            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();
            var token = tokenGenerator.Generate(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new { CurrentPassword = password, NewPassword = "NewP@ssw0rd123" };

            var response = await _httpClient.PatchAsJsonAsync("/users/password", request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var request = new { CurrentPassword = "any", NewPassword = "NewP@ssw0rd123" };

            var response = await _httpClient.PatchAsJsonAsync("/users/password", request);

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

            var request = new { CurrentPassword = "any", NewPassword = "NewP@ssw0rd123" };

            var response = await _httpClient.PatchAsJsonAsync("/users/password", request);

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
            (var user, var password) = await _factory.SeedUserAsync();

            var tokenGenerator = _factory.Services.GetRequiredService<ITokenGenerator>();
            var token = tokenGenerator.Generate(user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new { CurrentPassword = password, NewPassword = string.Empty };

            var response = await _httpClient.PatchAsJsonAsync("/users/password", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().NotBeEmpty();
        }
    }
}
