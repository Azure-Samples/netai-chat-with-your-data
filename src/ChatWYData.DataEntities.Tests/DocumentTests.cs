using ChatWYData.DataEntities;
using Xunit;

namespace ChatWYData.DataEntities.Tests;

public class DocumentTests
{
    [Fact]
    public void Document_Constructor_InitializesDefaultValues()
    {
        // Act
        var document = new Document();

        // Assert
        Assert.Equal(string.Empty, document.FileName);
        Assert.Equal(string.Empty, document.FileDescription);
        Assert.Equal("-", document.FileMarkDown);
        Assert.Equal(string.Empty, document.FileContentType);
        Assert.Equal(string.Empty, document.FileBlobUri);
        Assert.Equal(0, document.Chunks);
        Assert.True(document.CreatedAt <= DateTime.UtcNow);
        Assert.True(document.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Document_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var document = new Document();
        var testFileName = "test-document.pdf";
        var testDescription = "Test document description";
        var testMarkDown = "# Test Document";
        var testContentType = "application/pdf";
        var testBlobUri = "https://storage.blob.core.windows.net/container/test-document.pdf";
        var testChunks = 5;
        var testCreatedAt = DateTime.UtcNow.AddDays(-1);
        var testUpdatedAt = DateTime.UtcNow;

        // Act
        document.FileName = testFileName;
        document.FileDescription = testDescription;
        document.FileMarkDown = testMarkDown;
        document.FileContentType = testContentType;
        document.FileBlobUri = testBlobUri;
        document.Chunks = testChunks;
        document.CreatedAt = testCreatedAt;
        document.UpdatedAt = testUpdatedAt;

        // Assert
        Assert.Equal(testFileName, document.FileName);
        Assert.Equal(testDescription, document.FileDescription);
        Assert.Equal(testMarkDown, document.FileMarkDown);
        Assert.Equal(testContentType, document.FileContentType);
        Assert.Equal(testBlobUri, document.FileBlobUri);
        Assert.Equal(testChunks, document.Chunks);
        Assert.Equal(testCreatedAt, document.CreatedAt);
        Assert.Equal(testUpdatedAt, document.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("document.pdf")]
    [InlineData("long-document-name-with-special-chars_123.docx")]
    public void Document_FileName_AcceptsVariousFormats(string fileName)
    {
        // Arrange
        var document = new Document();

        // Act
        document.FileName = fileName;

        // Assert
        Assert.Equal(fileName, document.FileName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Document_Chunks_AcceptsValidValues(int chunks)
    {
        // Arrange
        var document = new Document();

        // Act
        document.Chunks = chunks;

        // Assert
        Assert.Equal(chunks, document.Chunks);
    }

    [Fact]
    public void Document_TimestampsInitializedWithinReasonableTimeframe()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var document = new Document();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(document.CreatedAt >= beforeCreation);
        Assert.True(document.CreatedAt <= afterCreation);
        Assert.True(document.UpdatedAt >= beforeCreation);
        Assert.True(document.UpdatedAt <= afterCreation);
    }
}