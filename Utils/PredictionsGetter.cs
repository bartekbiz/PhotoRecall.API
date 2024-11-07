using System.Collections.Concurrent;
using Data;
using Data.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Utils;

public class PredictionsGetter(ILogger logger, List<YoloRunnerConfig> yoloRunnersConfig)
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<List<YoloRunnerResultDto>> GetPredictions(string photoUrl)
    {
        var result = new List<YoloRunnerResultDto>();
        
        await Parallel.ForEachAsync(yoloRunnersConfig, async (yoloRunner, cancellationToken) =>
        {
            var modelsToRun = new ConcurrentQueue<string>(yoloRunner.Models);
            
            await Parallel.ForEachAsync(yoloRunner.Urls, cancellationToken, async (url, token) =>
            {
                while (!modelsToRun.IsEmpty)
                {
                    if (!modelsToRun.TryDequeue(out var model))
                        continue;

                    var requestResult = await RequestYoloRunner(url, photoUrl, model, token);
                    
                    var yoloRunnerResult = new YoloRunnerResultDto()
                    {
                        YoloRunnerInfo = new YoloRunnerInfoDto()
                        {
                            Name = yoloRunner.Name,
                            Url = url,
                            Model = model
                        },
                        Predictions = requestResult,
                    };
                
                    lock (result)
                    {
                        result.Add(yoloRunnerResult);
                    }
                }
            });
        });

        return result;
    }

    private async Task<List<PredictionDto>?> RequestYoloRunner(string url, string photoUrl, string model,
        CancellationToken token)
    {
        List<PredictionDto>? result = null;
        
        try
        {
            var uri = CreateRequestUri(url, photoUrl, model);
            var response = await _client.GetAsync(uri, token);

            var responseContent = await response.Content.ReadAsStringAsync(token);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Request to YoloRunner was not successful, url: {url}, model: {model}, info: {response}");
            }

            if (responseContent == string.Empty)
            {
                logger.LogError($"Prediction data from YoloRunner was empty, url: {url}, model: {model}, info: {response}");
            }

            result = JsonConvert.DeserializeObject<List<PredictionDto>>(responseContent);
        }
        catch (Exception e)
        {
            logger.LogError($"Request to YoloRunner was not successfull, info: {e}");
        }
        
        return result;
    }

    private static Uri CreateRequestUri(string url, string photoUrl, string model)
    {
        var uri = $"{url}/predict";
        
        var queryParams = new Dictionary<string, string?>
        {
            { "photo_url", photoUrl },
            { "model_name", model }
        };

        return new Uri(QueryHelpers.AddQueryString(uri, queryParams));
    }
}