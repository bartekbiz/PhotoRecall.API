using System.Collections.Concurrent;
using Data;
using Data.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Utils;

public class PredictionsGetter
{
    private readonly ILogger _logger;
    private readonly HttpClient _client = new HttpClient();
    
    public PredictionsGetter(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task<List<YoloRunnerResultDto>> RunYoloRunners(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        var result = new List<YoloRunnerResultDto>();
        
        await Parallel.ForEachAsync(yoloRunnersConfig, 
            async (yoloRunner, cancellationToken) =>
        {
            var modelsToRun = new ConcurrentQueue<string>(yoloRunner.Models);
            
            await Parallel.ForEachAsync(yoloRunner.Urls, cancellationToken, 
                async (url, cancellationToken) =>
            {
                while (!modelsToRun.IsEmpty)
                {
                    if (!modelsToRun.TryDequeue(out var model))
                        continue;

                    var requestResult = await GetPredictions(url, model, cancellationToken);
                    
                    var yoloRunnerResult = new YoloRunnerResultDto()
                    {
                        YoloRunnerInfo = new YoloRunnerInfoDto()
                        {
                            Name = yoloRunner.Name,
                            Url = url,
                            Model = model
                        },
                        Predictions = requestResult.Item1,
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

    private async Task<(List<PredictionDto>?, string?)> GetPredictions(string url, string model, 
        CancellationToken cancellationToken)
    {
        List<PredictionDto>? result = null;
        string? errorMessage = null;
        
        try
        {
            var response = await _client.GetAsync(CreateRequestUri(url, model), cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Request to YoloRunner was not successful, url: {url}, model: {model}, info: {response}");
            }

            if (responseContent == string.Empty)
            {
                _logger.LogError($"Prediction data from YoloRunner was empty, url: {url}, model: {model}, info: {response}");
            }

            result = JsonConvert.DeserializeObject<List<PredictionDto>>(responseContent);
        }
        catch (Exception e)
        {
            _logger.LogError($"Request to YoloRunner was not successfull, info: {e}");
        }
        
        return (result, errorMessage);
    }

    private static Uri CreateRequestUri(string url, string model)
    {
        var uri = $"{url}/predict";
        
        var queryParams = new Dictionary<string, string?>
        {
            { "photo_url", "https://ultralytics.com/images/bus.jpg" },
            { "model_name", model }
        };

        return new Uri(QueryHelpers.AddQueryString(uri, queryParams));
    }
}