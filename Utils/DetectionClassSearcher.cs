using Data.Configuration;
using Data.Dtos;
using Data.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Utils;

public class DetectionClassSearcher
{
    private SynonymsConfig _synonymsConfig;

    public DetectionClassSearcher(SynonymsConfig synonymsConfig)
    {
        _synonymsConfig = synonymsConfig;
    }
    
    public async Task<List<DetectionClassEnum>> Search(string input)
    {
        var result = new List<DetectionClassEnum>();
        var phrasesToCheck = new List<string> { input };
        phrasesToCheck.AddRange(await GetSynonyms(input));
        
        foreach (var phrase in phrasesToCheck)
        {
            var map = OtherUtils.TryMapPhraseToDetectionClass(phrase);
            if (map == null) continue;
            
            result.Add(map.Value);
        }

        return result.Distinct().ToList();
    }

    private async Task<List<string>> GetSynonyms(string phrase)
    {
        var client = new HttpClient();
        
        var query = new Dictionary<string, string> { ["word"] = phrase };
        var uri = QueryHelpers.AddQueryString(_synonymsConfig.Uri, query);
        
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add("X-Api-Key", _synonymsConfig.Key.Trim());
        
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }
        
        var result = JsonConvert.DeserializeObject<SynonymsApiDto>(await response.Content.ReadAsStringAsync());
        return result?.Synonyms ?? [];
    }
}