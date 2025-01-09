using Data.Enums;
using Utils;
using Xunit.Abstractions;

namespace UnitTests.Tests;

public class OtherUtilsTests
{
    private readonly ITestOutputHelper _output;
    
    public OtherUtilsTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void MapPhraseToEnum_MapsToEnumCorrectly()
    {
        // Arrange
        var enumList = Enum
            .GetValues(typeof(DetectionClassEnum))
            .Cast<DetectionClassEnum>()
            .ToList();

        var rnd = new Random();
        var testingValues = new Dictionary<int, string> { {-1, ""} };

        for (var i = 0; i < TestConfig.NumberOfRandomTests; i++)
        {
            var rndNumber = rnd.Next(0, enumList.Count);

            string enumName = enumList[rndNumber].ToString();
            int enumValue = (int)Enum.Parse(typeof(DetectionClassEnum), enumName);
            
            testingValues.TryAdd(enumValue, enumName);
        }

        foreach (var kvp in testingValues)
        {
            // Act
            var result = OtherUtils.TryMapPhraseToEnum<DetectionClassEnum>(kvp.Value);
            
            // Assert
            if (result == null)
            {
                Assert.DoesNotContain(enumList, value => value == (DetectionClassEnum)kvp.Key);
                continue;
            }
            
            Assert.Equal(kvp.Key, (int)result);
        }
    }
}