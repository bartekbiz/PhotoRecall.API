using Utils;
using Xunit.Abstractions;

namespace UnitTests.Tests;

public class FileUtilsTests
{
    private readonly ITestOutputHelper _output;
    
    public FileUtilsTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void CreateRandomFileName_CreatesDifferentFileNameWithSameExtension()
    {
        // Arrange
        const string name = "photo";
        const string extension = ".jpg";
        string fileName = name + extension;
        
        // Act
        string result = FileUtils.CreateRandomFileName(fileName);

        // Assert
        Assert.NotEqual(fileName, result);
        Assert.Equal(extension, "." + result.Split(".").Last());
    }
}