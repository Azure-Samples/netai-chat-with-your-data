using ChatWYData.ApiServices;
using ChatWYData.DataEntities;
using ChatWYData.FileEntities;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ChatWYData.ApiServices.Tests;

public class DocumentApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly DocumentApiService _documentApiService;

    public DocumentApiServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };
        _documentApiService = new DocumentApiService(_httpClient);
    }

    [Fact]
    public void DocumentApiService_Constructor_AcceptsHttpClient()
    {
        // Arrange & Act
        var service = new DocumentApiService(_httpClient);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetDocumentsAsync_ReturnsDocumentsList_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedDocuments = new List<Document>
        {
            new() { FileName = "doc1.pdf", FileDescription = "First document" },
            new() { FileName = "doc2.pdf", FileDescription = "Second document" }
        };

        var jsonResponse = JsonSerializer.Serialize(expectedDocuments);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("/docs/getall")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.GetDocumentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("doc1.pdf", result[0].FileName);
        Assert.Equal("First document", result[0].FileDescription);
        Assert.Equal("doc2.pdf", result[1].FileName);
        Assert.Equal("Second document", result[1].FileDescription);
    }

    [Fact]
    public async Task GetDocumentsAsync_ReturnsEmptyList_WhenResponseFails()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _documentApiService.GetDocumentsAsync());

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetDocumentAsync_ReturnsDocument_WhenResponseIsSuccessful()
    {
        // Arrange
        var fileName = "test.pdf";
        var expectedDocument = new Document 
        { 
            FileName = fileName, 
            FileDescription = "Test document",
            FileContentType = "application/pdf"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedDocument);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/docs/get/{fileName}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.GetDocumentAsync(fileName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileName, result.FileName);
        Assert.Equal("Test document", result.FileDescription);
        Assert.Equal("application/pdf", result.FileContentType);
    }

    [Fact]
    public async Task GetDocumentAsync_ReturnsNewDocument_WhenResponseFails()
    {
        // Arrange
        var fileName = "test.pdf";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _documentApiService.GetDocumentAsync(fileName));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetDocumentUriAsync_ReturnsUri_WhenResponseIsSuccessful()
    {
        // Arrange
        var fileName = "test.pdf";
        var expectedUri = "https://storage.blob.core.windows.net/container/test.pdf";

        var jsonResponse = JsonSerializer.Serialize(expectedUri);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/docs/getDocUri/{fileName}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.GetDocumentUriAsync(fileName);

        // Assert
        Assert.Equal(expectedUri, result);
    }

    [Fact]
    public async Task GetDocumentUriAsync_ReturnsEmptyString_WhenResponseFails()
    {
        // Arrange
        var fileName = "test.pdf";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _documentApiService.GetDocumentUriAsync(fileName));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task FileBatchUploadDocAsync_ReturnsResponse_WhenSuccessful()
    {
        // Arrange
        var request = new FileUploadRequest
        {
            FileName = "test.pdf",
            FileContentType = "application/pdf",
            FileBytes = Encoding.UTF8.GetBytes("test content")
        };

        var expectedResponse = new FileBatchUploadResponse
        {
            FileName = "test.pdf"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri!.ToString().EndsWith("/filebatch/upload") &&
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.FileBatchUploadDocAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.pdf", result.FileName);
    }

    [Fact]
    public async Task IgnoreSingleDocumentAsync_ReturnsTrue_WhenResponseIsTrue()
    {
        // Arrange
        var fileName = "test.pdf";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("true", Encoding.UTF8, "text/plain")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/filebatch/updateignore/{fileName}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.IgnoreSingleDocumentAsync(fileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IgnoreSingleDocumentAsync_ReturnsFalse_WhenResponseIsFalse()
    {
        // Arrange
        var fileName = "test.pdf";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("false", Encoding.UTF8, "text/plain")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/filebatch/updateignore/{fileName}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.IgnoreSingleDocumentAsync(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IgnoreSingleDocumentAsync_ReturnsFalse_WhenResponseIsInvalid()
    {
        // Arrange
        var fileName = "test.pdf";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid", Encoding.UTF8, "text/plain")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/filebatch/updateignore/{fileName}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.IgnoreSingleDocumentAsync(fileName);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFileBatchUploadsToProcessByProcessed_CallsCorrectEndpoint(bool processed)
    {
        // Arrange
        var expectedBatches = new List<FileProcessBatch>
        {
            new() { FileName = "test.pdf", Processed = processed }
        };

        var jsonResponse = JsonSerializer.Serialize(expectedBatches);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith($"/filebatch/getallbyProcessed/{processed}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _documentApiService.GetFileBatchUploadsToProcessByProcessed(processed);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(processed, result[0].Processed);
    }
}