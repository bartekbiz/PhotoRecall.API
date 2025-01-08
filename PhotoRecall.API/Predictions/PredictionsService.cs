using Data;
using Data.Dtos;
using Microsoft.Extensions.Options;
using PhotoRecall.API.Exceptions;
using PhotoRecall.API.Info;
using Utils;
using Utils.PredictionsMergers;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private readonly ILogger<PredictionsService> _logger;
    private readonly IInfoService _infoService;
    private readonly List<ModelRunnerConfig> _modelRunnersConfig;
    private readonly PhotosConfig _photosConfig;
    private readonly UrlsConfig _urlsConfig;
    private readonly PredictionsGetter _predictionsGetter;

    public PredictionsService(ILogger<PredictionsService> logger,
        IInfoService infoService,
        IOptions<List<ModelRunnerConfig>> yoloRunnersConfig,
        IOptions<PhotosConfig> photosConfig,
        IOptions<UrlsConfig> urlsConfig)
    {
        _logger = logger;
        _infoService = infoService;
        _modelRunnersConfig = yoloRunnersConfig.Value;
        ValidateYoloRunnersConfig(_modelRunnersConfig);
        _photosConfig = photosConfig.Value;
        _urlsConfig = urlsConfig.Value;
        _predictionsGetter = new PredictionsGetter(logger, _modelRunnersConfig);
    }
    
    #region GetMergedPredictions
   
    public async Task<List<PredictionDtoMerged>> GetPredictionsAllDetectedAsync(PredictionPropsDto propsDto)
    {
        var modelCount = GetModelsList(propsDto).Count;
        var threshold = (propsDto.AgreeRatio / 100) * modelCount;

        return await GetMergedPredictions(PredictionsMergerType.AllDetected, propsDto, threshold);
    }
    
    public async Task<List<PredictionDtoMerged>> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto)
    {
        return await GetMergedPredictions(PredictionsMergerType.VotedWithCounts, propsDto, new object());
    }
    
    private async Task<List<PredictionDtoMerged>> GetMergedPredictions(PredictionsMergerType mergerType, 
        PredictionPropsDto propsDto, object args)
    {
        var predictions = await GetPredictionsAsync(propsDto);

        var predictionsMerger = PredictionsMergerFactory.Create(mergerType);
        
        try
        {
            if (predictionsMerger == null)
            {
                throw new Exception();
            }
            
            return predictionsMerger.Merge(predictions, args);
        }
        catch
        {
            throw new Exception("Could not merge predictions :(");
        }
    }
    
    #endregion
    
    #region GetPredictions
    
    public async Task<List<ModelRunResultDto>> GetAllPredictionsAsync(IFormFile photo)
    {
        var modelsList = _infoService.GetAvailableYoloModels();
        ValidatePhoto(photo);
        
        return await GetPredictionsAsync(photo, modelsList);
    }
    
    public async Task<List<ModelRunResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto)
    {
        var modelsList = GetModelsList(propsDto);
        
        ValidateProps(propsDto.Photo, modelsList);

        return await GetPredictionsAsync(propsDto.Photo, modelsList!);
    }

    private async Task<List<ModelRunResultDto>> GetPredictionsAsync(IFormFile photo, List<string> modelsList)
    {
        var hostedPhoto = await FileUtils
            .SaveAndHostFile(_photosConfig.Path, _urlsConfig.ContainerUrl, photo);

        var predictions = await _predictionsGetter
            .GetPredictions(hostedPhoto.Uri, modelsList);

        FileUtils.DeleteFile(hostedPhoto.Path);

        return predictions;
    }
    
    #endregion
    
    #region Utils
    
    #region ValidateYoloRunnerConfig

    private void ValidateYoloRunnersConfig(List<ModelRunnerConfig> yoloRunnersConfig)
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
    
    #region ValidateProps

    private void ValidateProps(IFormFile photo, List<string>? modelsList)
    {
        ValidatePhoto(photo);
        
        if (modelsList == null || modelsList.Count == 0)
        {
            throw new BadRequestException("YoloModels should be passed as a valid json string.");
        }
    }

    private void ValidatePhoto(IFormFile photo)
    {
        var fileExtension = photo.FileName.Split(".").Last().ToLower();
        if (!_photosConfig.AcceptedTypes.Contains(fileExtension))
        {
            throw new BadRequestException($"File type \".{fileExtension}\" is not supported.");
        }
    }

    #endregion
    
    private List<string> GetModelsList(PredictionPropsDto propsDto)
    {
        var modelsListProps = string.IsNullOrEmpty(propsDto.YoloModels) ?
            null : 
            OtherUtils.ConvertJsonStringToList(propsDto.YoloModels);

        return modelsListProps ?? _infoService.GetAvailableYoloModels();
    }

    #endregion
}