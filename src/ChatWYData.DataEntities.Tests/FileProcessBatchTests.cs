using ChatWYData.DataEntities;
using Xunit;

namespace ChatWYData.DataEntities.Tests;

public class FileProcessBatchTests
{
    [Fact]
    public void FileProcessBatch_Constructor_InitializesDefaultValues()
    {
        // Act
        var batch = new FileProcessBatch();

        // Assert
        Assert.Equal(string.Empty, batch.FileName);
        Assert.Equal(string.Empty, batch.FileBlobUri);
        Assert.Equal(string.Empty, batch.FileContentType);
        Assert.Equal(string.Empty, batch.StatusMessage);
        Assert.Equal(string.Empty, batch.StepsDescription);
        Assert.False(batch.Processed);
        Assert.False(batch.Ignore);
        Assert.True(batch.CreatedDate <= DateTime.UtcNow);
        Assert.True(batch.LastModifiedDate <= DateTime.UtcNow);
        Assert.Equal(DateTime.MaxValue, batch.CompletedDate);
    }

    [Fact]
    public void FileProcessBatch_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var batch = new FileProcessBatch();
        var testFileName = "test-file.pdf";
        var testBlobUri = "https://storage.blob.core.windows.net/container/test-file.pdf";
        var testContentType = "application/pdf";
        var testStatusMessage = "Processing completed successfully";
        var testStepsDescription = "Step 1: Extract text\nStep 2: Generate embeddings";
        var testCreatedDate = DateTime.UtcNow.AddHours(-2);
        var testLastModifiedDate = DateTime.UtcNow.AddMinutes(-30);
        var testCompletedDate = DateTime.UtcNow;

        // Act
        batch.FileName = testFileName;
        batch.FileBlobUri = testBlobUri;
        batch.FileContentType = testContentType;
        batch.StatusMessage = testStatusMessage;
        batch.StepsDescription = testStepsDescription;
        batch.Processed = true;
        batch.Ignore = true;
        batch.CreatedDate = testCreatedDate;
        batch.LastModifiedDate = testLastModifiedDate;
        batch.CompletedDate = testCompletedDate;

        // Assert
        Assert.Equal(testFileName, batch.FileName);
        Assert.Equal(testBlobUri, batch.FileBlobUri);
        Assert.Equal(testContentType, batch.FileContentType);
        Assert.Equal(testStatusMessage, batch.StatusMessage);
        Assert.Equal(testStepsDescription, batch.StepsDescription);
        Assert.True(batch.Processed);
        Assert.True(batch.Ignore);
        Assert.Equal(testCreatedDate, batch.CreatedDate);
        Assert.Equal(testLastModifiedDate, batch.LastModifiedDate);
        Assert.Equal(testCompletedDate, batch.CompletedDate);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void FileProcessBatch_BooleanFlags_WorkCorrectly(bool processed, bool ignore)
    {
        // Arrange
        var batch = new FileProcessBatch();

        // Act
        batch.Processed = processed;
        batch.Ignore = ignore;

        // Assert
        Assert.Equal(processed, batch.Processed);
        Assert.Equal(ignore, batch.Ignore);
    }

    [Fact]
    public void FileProcessBatch_StatusAndStepsDescription_CanContainMultilineText()
    {
        // Arrange
        var batch = new FileProcessBatch();
        var multilineStatus = "Processing failed:\nError 1: Invalid format\nError 2: Missing metadata";
        var multilineSteps = "Step 1: Upload file\nStep 2: Extract content\nStep 3: Process embeddings\nStep 4: Store in vector database";

        // Act
        batch.StatusMessage = multilineStatus;
        batch.StepsDescription = multilineSteps;

        // Assert
        Assert.Equal(multilineStatus, batch.StatusMessage);
        Assert.Equal(multilineSteps, batch.StepsDescription);
        Assert.Contains("\n", batch.StatusMessage);
        Assert.Contains("\n", batch.StepsDescription);
    }

    [Fact]
    public void FileProcessBatch_DateProperties_HandleTimeZones()
    {
        // Arrange
        var batch = new FileProcessBatch();
        var utcTime = DateTime.UtcNow;
        var specificUtcTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);

        // Act
        batch.CreatedDate = specificUtcTime;
        batch.LastModifiedDate = utcTime;
        batch.CompletedDate = specificUtcTime.AddHours(2);

        // Assert
        Assert.Equal(specificUtcTime, batch.CreatedDate);
        Assert.Equal(utcTime, batch.LastModifiedDate);
        Assert.Equal(specificUtcTime.AddHours(2), batch.CompletedDate);
    }

    [Fact]
    public void FileProcessBatch_TimestampsInitializedWithinReasonableTimeframe()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var batch = new FileProcessBatch();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(batch.CreatedDate >= beforeCreation);
        Assert.True(batch.CreatedDate <= afterCreation);
        Assert.True(batch.LastModifiedDate >= beforeCreation);
        Assert.True(batch.LastModifiedDate <= afterCreation);
    }

    [Theory]
    [InlineData("")]
    [InlineData("application/pdf")]
    [InlineData("text/plain")]
    [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    public void FileProcessBatch_FileContentType_AcceptsVariousMediaTypes(string contentType)
    {
        // Arrange
        var batch = new FileProcessBatch();

        // Act
        batch.FileContentType = contentType;

        // Assert
        Assert.Equal(contentType, batch.FileContentType);
    }
}