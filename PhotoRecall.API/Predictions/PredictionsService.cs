using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PhotoRecall.API.Configuration;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private static readonly HttpClient Client = new HttpClient();
    
    public async Task<List<PredictionDto>> GetPredictionsAsync()
    {
        var result = new List<PredictionDto>();
        
        var yoloModulesConfig = GetYoloModulesConfiguration();

        if (yoloModulesConfig == null)
        {
            throw new Exception("Could not read yolo modules configuration.");
        }

        await Parallel.ForEachAsync(yoloModulesConfig, async (config, cancellationToken) =>
        {
            var singleResult = await GetResultsFromYoloModel(config, cancellationToken);
            
            lock (result)
            {
                result.AddRange(singleResult);
            }
        });
        
        return result;
    }

    private async Task<List<PredictionDto>> GetResultsFromYoloModel(YoloModuleConfiguration moduleConfig, 
        CancellationToken cancellationToken)
    {
        var uri = BuildRequestUri(moduleConfig.Url, "https://ultralytics.com/images/bus.jpg");
            
        var response = await Client.GetStringAsync(uri, cancellationToken);
        if (response == string.Empty)
        {
            throw new HttpRequestException("No response");
        }
            
        var result = JsonConvert.DeserializeObject<List<PredictionDto>>(response);
        if (result is null)
        {
            throw new SerializationException();
        }

        return result;
    }

    private List<YoloModuleConfiguration> GetYoloModulesConfiguration()
    {
        var result = new List<YoloModuleConfiguration>();
        
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        result = config.GetRequiredSection("YoloModules").Get<List<YoloModuleConfiguration>>();
        
        return result;
    }

    private string BuildRequestUri(string moduleUrl, string photoUrl)
    {
        return $"{moduleUrl}/predict?photo_url={photoUrl}";
    }
}