using Data.Enums;

namespace PhotoRecall.API.Search;

public interface ISearchService
{
    Task<List<YoloClass>> GetYoloClassesAsync(string phrase);
}