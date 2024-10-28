using Data;
using Data.Dtos;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Predictions;

public class PredictionsService(
    ILogger<PredictionsService> logger,
    IOptions<List<YoloRunnerConfig>> yoloRunnersConfig,
    IOptions<PathsConfig> pathsConfig)
    : IPredictionsService
{
    private readonly List<YoloRunnerConfig> _yoloRunnersConfig = yoloRunnersConfig.Value;
    private readonly PathsConfig _pathsConfig = pathsConfig.Value;

    public async Task<List<PredictionDto>> GetVotedPredictionsAsync(IFormFile photo)
    {
        ValidateYoloRunnersConfig(_yoloRunnersConfig);
        await FileUtils.SaveFormFile(_pathsConfig.PhotosPath, photo);

        var predictionsGetter = new PredictionsGetter(logger);
        var predictions = await predictionsGetter.RunYoloRunners(_yoloRunnersConfig);

        return PredictionsProcessor.MergeByVoting(predictions);
    }
    
    public async Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(IFormFile photo)
    {
        ValidateYoloRunnersConfig(_yoloRunnersConfig);
        await FileUtils.SaveFormFile(_pathsConfig.PhotosPath, photo);
        
        var predictionsGetter = new PredictionsGetter(logger);
        return await predictionsGetter.RunYoloRunners(_yoloRunnersConfig);
    }

    #region ValidateYoloRunnerConfig

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

    #endregion
}