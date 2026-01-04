using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Web.Api.Models;
using Web.Api;

namespace Web.Api.Tests.Integration;

public class PingEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PingEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PingEndpoint_ReturnsPong()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/ping");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Should().Be("pong");
    }
}