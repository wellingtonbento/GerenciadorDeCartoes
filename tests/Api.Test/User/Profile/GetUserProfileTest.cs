using CardManager.Communication.Responses;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.Test.User.Profile
{
    public class GetUserProfileTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public GetUserProfileTest(CustomWebApplicationFactory factory)
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

            var response = await _httpClient.GetAsync("/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.Content.ReadFromJsonAsync<ResponseUserProfileJson>();

            responseData.Should().NotBeNull();
            responseData!.Name.Should().Be(user.Name);
            responseData.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task Error_No_Token()
        {
            var response = await _httpClient.GetAsync("/users");

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

            var response = await _httpClient.GetAsync("/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle()
                .And.Contain(error => error.GetString()!.Equals(MessagesException.ACCESS_DENIED));
        }
    }
}
