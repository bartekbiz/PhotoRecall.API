using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Serialization;
using Data;
using Data.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using PredictionUtils;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private static readonly HttpClient Client = new HttpClient();
    
    public async Task<List<PredictionDto>> GetVotedPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        ValidateYoloRunnersConfig(yoloRunnersConfig);

        var predictions = await RunYoloRunners(yoloRunnersConfig);

        return PredictionsProcessor.MergeByVoting(predictions);
    }
    
    public async Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        ValidateYoloRunnersConfig(yoloRunnersConfig);

        return await RunYoloRunners(yoloRunnersConfig);
    }

    #region ValidateYoloRunnersConfig

    private static void ValidateYoloRunnersConfig(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        if (yoloRunnersConfig is not { Count: > 0 })
        {
            throw new Exception("YoloRunners configuration is empty.");
        }

        if (yoloRunnersConfig.Any(a => a.Addresses?.Count == 0))
        {
            throw new Exception("One or more YoloRunners have invalid url configuration.");
        }
    }

    #endregion
    
    private static async Task<List<YoloRunnerResultDto>> RunYoloRunners(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        var result = new List<YoloRunnerResultDto>();
        
        await Parallel.ForEachAsync(yoloRunnersConfig, 
            async (yoloRunner, cancellationToken) =>
        {
            var modelsToRun = yoloRunner.Models.ConvertAll(model => model);
            
            await Parallel.ForEachAsync(yoloRunner.Addresses, cancellationToken, 
                async (address, cancellationToken) =>
            {
                while (modelsToRun.Count > 0)
                {
                    string model;
                    lock (modelsToRun)
                    {
                        var modelNullable = modelsToRun.FirstOrDefault();

                        if (modelNullable == null)
                            continue;
                        
                        model = modelNullable;
                        modelsToRun.RemoveAt(0);
                    }
                    
                    var yoloModelPredictions = new YoloRunnerResultDto()
                    {
                        YoloRunnerExecution = new YoloRunnerExecutionDto()
                        {
                            Name = yoloRunner.Name,
                            Address = address,
                            Model = model
                        },
                        Predictions = await GetResultsFromYoloModel(address, model, cancellationToken)
                    };
                
                    lock (result)
                    {
                        result.Add(yoloModelPredictions);
                    }
                }
            });
        });

        return result;
    }

    private static async Task<List<PredictionDto>> GetResultsFromYoloModel(string address, string model, 
        CancellationToken cancellationToken)
    {
        var uri = $"http://{address}/predict";
        
        var queryParams = new Dictionary<string, string?>
        {
            { "photo_url", "https://ultralytics.com/images/bus.jpg" },
            { "model_name", model }
        };

        var finalUri = new Uri(QueryHelpers.AddQueryString(uri, queryParams));
        
        var response = await Client.GetAsync(finalUri, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode || responseContent == string.Empty)
        {
            // Log
            return [];
        }
            
        var result = JsonConvert.DeserializeObject<List<PredictionDto>>(responseContent);
        if (result is null)
        {
            // Log
            return [];
        }

        return result;
    }
}