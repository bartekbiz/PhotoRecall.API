using Data;
using Data.Configuration;
using Data.Dtos;
using Data.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Utils;

public class YoloClassSearcher : ClassSearcher<YoloClass>
{
    public YoloClassSearcher(SynonymsConfig synonymsConfig) : base(synonymsConfig)
    { }
    
    public override async Task<List<YoloClass>> Search(string input)
    {
        var result = new List<YoloClass>();
        var phrasesToCheck = new List<string> { input };
        phrasesToCheck.AddRange(await GetSynonyms(input));
        
        foreach (var phrase in phrasesToCheck)
        {
            var map = MapPhraseToClass(phrase);
            if (map == null) continue;
            
            result.Add(map.Value);
        }

        return result.Distinct().ToList();
    }

    private async Task<List<string>> GetSynonyms(string phrase)
    {
        var client = new HttpClient();
        
        var query = new Dictionary<string, string> { ["word"] = phrase };
        var uri = QueryHelpers.AddQueryString(SynonymsConfig.Uri, query);
        
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add("X-Api-Key", SynonymsConfig.Key.Trim());
        
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }
        
        var result = JsonConvert.DeserializeObject<SynonymsApiDto>(await response.Content.ReadAsStringAsync());
        return result?.Synonyms ?? [];
    }
    
    private YoloClass? MapPhraseToClass(string phrase)
    {
        phrase = phrase.Trim().Replace(" ", "").ToLower();
        
        if (Enum.TryParse(phrase, ignoreCase: true, out YoloClass yoloClass))
        {
            return yoloClass;
        }
        
        return null;
    }
}