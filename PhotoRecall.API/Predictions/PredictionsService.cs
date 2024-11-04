using Data;
using Data.Dtos;
using Microsoft.Extensions.Options;
using Utils;
using Utils.PredictionsMergers;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private readonly ILogger<PredictionsService> _logger;
    private readonly List<YoloRunnerConfig> _yoloRunnersConfig;
    private readonly PathsConfig _pathsConfig;
    private readonly UrlsConfig _urlsConfig;
    private readonly PredictionsGetter _predictionsGetter;

    public PredictionsService(ILogger<PredictionsService> logger,
        IOptions<List<YoloRunnerConfig>> yoloRunnersConfig,
        IOptions<PathsConfig> pathsConfig,
        IOptions<UrlsConfig> urlsConfig)
    {
        _logger = logger;
        _yoloRunnersConfig = yoloRunnersConfig.Value;
        _pathsConfig = pathsConfig.Value;
        _urlsConfig = urlsConfig.Value;
        _predictionsGetter = new PredictionsGetter(logger, _yoloRunnersConfig);
    }

    public async Task<List<PredictionWithCountDto>> GetVotedPredictionsWithCountAsync(IFormFile photo)
    {
        var predictions = await GetAllPredictionsAsync(photo);

        var predictionsMerger = new PredictionsMergerWithCounts(predictions);
        return predictionsMerger.Merge();
    }
    
    public async Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(IFormFile photo)
    {
        ValidateYoloRunnersConfig(_yoloRunnersConfig);
        
        var hostedPhoto = await FileUtils
            .SaveAndHostFile(_pathsConfig.PhotosPath, _urlsConfig.ContainerUrl, photo);
        
        var predictions = await _predictionsGetter
            .RunYoloRunners(hostedPhoto.Uri);
        
        FileUtils.DeleteFile(hostedPhoto.Path);

        return predictions;
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