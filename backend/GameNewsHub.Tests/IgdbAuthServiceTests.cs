using System.Net;
using backend.Configuration;
using GameNewsHub.Api.External;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace GameNewsHub.Tests;

public class IgdbAuthServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly IgdbAuthService _service;

    public IgdbAuthServiceTests()
    {
        var config = Options.Create(new ApiConfig
        {
            BaseUrl = "http://test.com",
            ClientId = "id",
            ClientSecret = "secret",
            TokenUrl = "http://token.com"
        });

        _handlerMock = new Mock<HttpMessageHandler>();

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"access_token\": \"test_token\", \"expires_in\": 3600 }")
            });

        var httpClient = new HttpClient(_handlerMock.Object);
        var logger = new Mock<ILogger<IgdbAuthService>>().Object;
        _service = new IgdbAuthService(config, httpClient, logger);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldFetchToken_WhenCacheIsEmpty()
    {

        var response = await _service.GetAccessTokenAsync();

        Assert.NotNull(response);
        Assert.Equal("test_token", response);
    }
    
    [Fact]
    public async Task GetAccessTokenAsync_ShouldReturnToken_WhenTokenIsValid()
    {
        var firstResponse = await _service.GetAccessTokenAsync();
        var secondResponse = await _service.GetAccessTokenAsync();
        
        Assert.Equal(firstResponse, secondResponse);

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldRefreshToken_WhenExpired()
    {
        _handlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"access_token\": \"old\", \"expires_in\": -10 }")
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"access_token\": \"new\", \"expires_in\": 3600 }")
            });

        await _service.GetAccessTokenAsync();
        var result = await _service.GetAccessTokenAsync();
        
        Assert.Equal("new", result);
        
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(2),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
    


}