using System.Collections.Concurrent;
using Data;
using Data.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace PredictionUtils;

public class PredictionsGetter
{
    private readonly HttpClient _client = new HttpClient();

    public PredictionsGetter()
    {
        
    }
    
    public async Task<List<YoloRunnerResultDto>> RunYoloRunners(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        var result = new List<YoloRunnerResultDto>();
        
        await Parallel.ForEachAsync(yoloRunnersConfig, 
            async (yoloRunner, cancellationToken) =>
        {
            var modelsToRun = new ConcurrentQueue<string>(yoloRunner.Models);
            
            await Parallel.ForEachAsync(yoloRunner.Addresses, cancellationToken, 
                async (address, cancellationToken) =>
            {
                while (!modelsToRun.IsEmpty)
                {
                    if (!modelsToRun.TryDequeue(out var model))
                        continue;
                    
                    var yoloPredictions = new YoloRunnerResultDto()
                    {
                        YoloRunnerInfo = new YoloRunnerInfoDto()
                        {
                            Name = yoloRunner.Name,
                            Address = address,
                            Model = model
                        },
                        Predictions = await GetPredictions(address, model, cancellationToken)
                    };
                
                    lock (result)
                    {
                        result.Add(yoloPredictions);
                    }
                }
            });
        });

        return result;
    }

    private async Task<List<PredictionDto>?> GetPredictions(string address, string model, 
        CancellationToken cancellationToken)
    {
        List<PredictionDto>? result = null;
        
        try
        {
            var response = await _client.GetAsync(CreateRequestUri(address, model), cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode || responseContent == string.Empty)
            {
                // Log
                return [];
            }

            result = JsonConvert.DeserializeObject<List<PredictionDto>>(responseContent);
        }
        catch (Exception e)
        {
            
        }
        
        return result;
    }

    private static Uri CreateRequestUri(string address, string model)
    {
        var uri = $"http://{address}/predict";
        
        var queryParams = new Dictionary<string, string?>
        {
            { "photo_url", "https://ultralytics.com/images/bus.jpg" },
            { "model_name", model }
        };

        return new Uri(QueryHelpers.AddQueryString(uri, queryParams));
    }
}