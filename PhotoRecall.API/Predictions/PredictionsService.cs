using Data;
using Data.Dtos;
using Microsoft.Extensions.Options;
using PredictionUtils;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private readonly ILogger<PredictionsService> _logger;
    private readonly List<YoloRunnerConfig> _yoloRunnersConfig;
    
    public PredictionsService(ILogger<PredictionsService> logger, IOptions<List<YoloRunnerConfig>> yoloRunnersConfig)
    {
        _logger = logger;
        _yoloRunnersConfig = yoloRunnersConfig.Value;
    }
    
    public async Task<List<PredictionDto>> GetVotedPredictionsAsync()
    {
        ValidateYoloRunnersConfig(_yoloRunnersConfig);

        var predictionsGetter = new PredictionsGetter(_logger);
        var predictions = await predictionsGetter.RunYoloRunners(_yoloRunnersConfig);

        return PredictionsProcessor.MergeByVoting(predictions);
    }
    
    public async Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync()
    {
        ValidateYoloRunnersConfig(_yoloRunnersConfig);
        
        var predictionsGetter = new PredictionsGetter(_logger);
        return await predictionsGetter.RunYoloRunners(_yoloRunnersConfig);
    }
    
    private void ValidateYoloRunnersConfig(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        if (yoloRunnersConfig is not { Count: > 0 })
        {
            throw new Exception("YoloRunners configuration is empty.");
        }

        if (yoloRunnersConfig.Any(a => a.Urls?.Count == 0))
        {
            throw new Exception("One or more YoloRunners have invalid url configuration.");
        }

        if (yoloRunnersConfig.Any(a => a.Models?.Count == 0))
        {
            throw new Exception("One or more YoloRunners have invalid models configuration.");
        }
    }
}