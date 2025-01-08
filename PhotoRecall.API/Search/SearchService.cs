using Data.Configuration;
using Data.Enums;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Search;

public class SearchService : ISearchService
{
    private readonly ApiConfig _apisConfig;

    public SearchService(IOptions<ApiConfig> apisConfig)
    {
        _apisConfig = apisConfig.Value;
    }
    
    public async Task<List<DetectionClassEnum>> GetDetectionClassesAsync(string phrase)
    {
        var yoloClassSearcher = new DetectionClassSearcher(_apisConfig.Synonyms);

        return await yoloClassSearcher.Search(phrase);
    }
}