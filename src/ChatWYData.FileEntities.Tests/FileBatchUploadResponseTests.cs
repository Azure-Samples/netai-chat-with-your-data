using ChatWYData.FileEntities;
using Xunit;

namespace ChatWYData.FileEntities.Tests;

public class FileBatchUploadResponseTests
{
    [Fact]
    public void FileBatchUploadResponse_Constructor_InitializesDefaultValues()
    {
        // Act
        var response = new FileBatchUploadResponse();

        // Assert
        Assert.Equal(string.Empty, response.FileName);
    }

    [Fact]
    public void FileBatchUploadResponse_FileName_CanBeSetAndRetrieved()
    {
        // Arrange
        var response = new FileBatchUploadResponse();
        var testFileName = "uploaded-document.pdf";

        // Act
        response.FileName = testFileName;

        // Assert
        Assert.Equal(testFileName, response.FileName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("simple.txt")]
    [InlineData("document with spaces.docx")]
    [InlineData("file-with-special-chars_123.xlsx")]
    [InlineData("very-long-filename-that-might-be-used-in-some-scenarios.pdf")]
    public void FileBatchUploadResponse_FileName_AcceptsVariousFormats(string fileName)
    {
        // Arrange
        var response = new FileBatchUploadResponse();

        // Act
        response.FileName = fileName;

        // Assert
        Assert.Equal(fileName, response.FileName);
    }

    [Fact]
    public void FileBatchUploadResponse_FileName_CanBeNull()
    {
        // Arrange
        var response = new FileBatchUploadResponse();

        // Act
        response.FileName = null!;

        // Assert
        Assert.Null(response.FileName);
    }

    [Fact]
    public void FileBatchUploadResponse_MultipleInstances_AreIndependent()
    {
        // Arrange
        var response1 = new FileBatchUploadResponse { FileName = "file1.pdf" };
        var response2 = new FileBatchUploadResponse { FileName = "file2.txt" };

        // Assert
        Assert.Equal("file1.pdf", response1.FileName);
        Assert.Equal("file2.txt", response2.FileName);
        Assert.NotEqual(response1.FileName, response2.FileName);
    }
}