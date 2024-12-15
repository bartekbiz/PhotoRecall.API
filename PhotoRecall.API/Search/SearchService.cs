using Data;
using Data.Configuration;
using Data.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Utils;

namespace PhotoRecall.API.Search;

public class SearchService : ISearchService
{
    private readonly ApiConfig _apisConfig;

    public SearchService(IOptions<ApiConfig> apisConfig)
    {
        _apisConfig = apisConfig.Value;
    }
    
    public async Task<List<YoloClass>> GetYoloClassesAsync(string phrase)
    {
        var yoloClassSearcher = new YoloClassSearcher(_apisConfig.Synonyms);

        return await yoloClassSearcher.Search(phrase);
    }
}