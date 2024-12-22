using Data.Enums;

namespace PhotoRecall.API.Search;

public interface ISearchService
{
    Task<List<YoloClassEnum>> GetYoloClassesAsync(string phrase);
}