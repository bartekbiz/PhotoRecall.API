using System;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using Utils;

namespace UnitTests.Tests;

public class UriUtilsTests
{
    [Fact]
    public void CreateUrl_WithSchemeAndHost_ReturnsCorrectUrl()
    {
        // Arrange
        string scheme = "https";
        string host = "example.com";

        // Act
        string result = UriUtils.CreateUrl(scheme, host);

        // Assert
        Assert.Equal("https://example.com", result);
    }

    [Fact]
    public void CreateUrl_WithHttpRequest_ReturnsCorrectUrl()
    {
        // Arrange
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Scheme).Returns("http");
        mockRequest.Setup(r => r.Host).Returns(new HostString("example.co"));

        // Act
        string result = UriUtils.CreateUrl(mockRequest.Object);

        // Assert
        Assert.Equal("http://example.co", result);
    }

    [Fact]
    public void CreateUri_WithUrlAndPath_ReturnsCorrectUri()
    {
        // Arrange
        string url = "https://example.net";
        string path = "api/values";

        // Act
        string result = UriUtils.CreateUri(url, path);

        // Assert
        Assert.Equal("https://example.net/api/values", result);
    }

    [Fact]
    public void CreateUri_WithHttpRequestAndPath_ReturnsCorrectUri()
    {
        // Arrange
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Scheme).Returns("https");
        mockRequest.Setup(r => r.Host).Returns(new HostString("example.org"));
        string path = "resource/42";

        // Act
        string result = UriUtils.CreateUri(mockRequest.Object, path);

        // Assert
        Assert.Equal("https://example.org/resource/42", result);
    }

    [Fact]
    public void CreateUri_WithSchemeHostAndPath_ReturnsCorrectUri()
    {
        // Arrange
        string scheme = "https";
        string host = "api.example.com";
        string path = "v1/users";

        // Act
        string result = UriUtils.CreateUri(scheme, host, path);

        // Assert
        Assert.Equal("https://api.example.com/v1/users", result);
    }
}
