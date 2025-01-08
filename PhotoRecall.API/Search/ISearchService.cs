using Data.Enums;

namespace PhotoRecall.API.Search;

public interface ISearchService
{
    Task<List<DetectionClassEnum>> GetDetectionClassesAsync(string phrase);
}