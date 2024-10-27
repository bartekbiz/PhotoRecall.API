using Data;
using Data.Dtos;
using PredictionUtils;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    public async Task<List<PredictionDto>> GetVotedPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        ValidateYoloRunnersConfig(yoloRunnersConfig);

        var predictionsGetter = new PredictionsGetter();
        var predictions = await predictionsGetter.RunYoloRunners(yoloRunnersConfig);

        return PredictionsProcessor.MergeByVoting(predictions);
    }
    
    public async Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig)
    {
        ValidateYoloRunnersConfig(yoloRunnersConfig);
        
        var predictionsGetter = new PredictionsGetter();
        return await predictionsGetter.RunYoloRunners(yoloRunnersConfig);
    }
    
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
}