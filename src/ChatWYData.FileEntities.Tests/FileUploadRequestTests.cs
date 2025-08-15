using ChatWYData.FileEntities;
using System.Text.Json;
using Xunit;

namespace ChatWYData.FileEntities.Tests;

public class FileUploadRequestTests
{
    [Fact]
    public void FileUploadRequest_Constructor_InitializesDefaultValues()
    {
        // Act
        var request = new FileUploadRequest();

        // Assert
        Assert.Equal(string.Empty, request.FileName);
        Assert.Equal(string.Empty, request.FileContentType);
        Assert.NotNull(request.FileBytes);
        Assert.Empty(request.FileBytes);
    }

    [Fact]
    public void FileUploadRequest_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var request = new FileUploadRequest();
        var testFileName = "document.pdf";
        var testContentType = "application/pdf";
        var testBytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        request.FileName = testFileName;
        request.FileContentType = testContentType;
        request.FileBytes = testBytes;

        // Assert
        Assert.Equal(testFileName, request.FileName);
        Assert.Equal(testContentType, request.FileContentType);
        Assert.Equal(testBytes, request.FileBytes);
        Assert.Equal(5, request.FileBytes.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test.txt")]
    [InlineData("document with spaces.docx")]
    [InlineData("file-with-dashes_and_underscores.xlsx")]
    public void FileUploadRequest_FileName_AcceptsVariousFormats(string fileName)
    {
        // Arrange
        var request = new FileUploadRequest();

        // Act
        request.FileName = fileName;

        // Assert
        Assert.Equal(fileName, request.FileName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("text/plain")]
    [InlineData("application/pdf")]
    [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("image/jpeg")]
    public void FileUploadRequest_FileContentType_AcceptsVariousMediaTypes(string contentType)
    {
        // Arrange
        var request = new FileUploadRequest();

        // Act
        request.FileContentType = contentType;

        // Assert
        Assert.Equal(contentType, request.FileContentType);
    }

    [Fact]
    public void FileUploadRequest_FileBytes_CanHandleLargeArrays()
    {
        // Arrange
        var request = new FileUploadRequest();
        var largeArray = new byte[10000];
        Random.Shared.NextBytes(largeArray);

        // Act
        request.FileBytes = largeArray;

        // Assert
        Assert.Equal(largeArray, request.FileBytes);
        Assert.Equal(10000, request.FileBytes.Length);
    }

    [Fact]
    public void FileUploadRequest_FileBytes_CanBeEmpty()
    {
        // Arrange
        var request = new FileUploadRequest();

        // Act
        request.FileBytes = Array.Empty<byte>();

        // Assert
        Assert.NotNull(request.FileBytes);
        Assert.Empty(request.FileBytes);
    }

    [Fact]
    public void FileUploadRequest_SerializesToJson_WithCorrectPropertyNames()
    {
        // Arrange
        var request = new FileUploadRequest
        {
            FileName = "test.pdf",
            FileContentType = "application/pdf",
            FileBytes = new byte[] { 1, 2, 3 }
        };

        // Act
        var json = JsonSerializer.Serialize(request);

        // Assert
        Assert.Contains("\"FileName\":", json);
        Assert.Contains("\"FileContentType\":", json);
        Assert.Contains("\"FileBytes\":", json);
        Assert.Contains("\"test.pdf\"", json);
        Assert.Contains("\"application/pdf\"", json);
    }

    [Fact]
    public void FileUploadRequest_DeserializesFromJson_WithCorrectPropertyNames()
    {
        // Arrange - Use base64 encoded string for byte array
        var json = """
        {
            "FileName": "test.pdf",
            "FileContentType": "application/pdf",
            "FileBytes": "AQIDBAU="
        }
        """;

        // Act
        var request = JsonSerializer.Deserialize<FileUploadRequest>(json);

        // Assert
        Assert.NotNull(request);
        Assert.Equal("test.pdf", request.FileName);
        Assert.Equal("application/pdf", request.FileContentType);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, request.FileBytes);
    }

    [Fact]
    public void FileUploadRequest_HandlesNullByteArray()
    {
        // Arrange
        var request = new FileUploadRequest();

        // Act & Assert - Should not throw
        request.FileBytes = null!;
        Assert.Null(request.FileBytes);
    }
}