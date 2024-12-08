using Data;
using Data.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhotoRecall.API.Exceptions;
using PhotoRecall.API.Info;
using Utils;
using Utils.PredictionsMergers;

namespace PhotoRecall.API.Predictions;

public class PredictionsService : IPredictionsService
{
    private readonly ILogger<PredictionsService> _logger;
    private readonly IInfoService _infoService;
    private readonly List<YoloRunnerConfig> _yoloRunnersConfig;
    private readonly PhotosConfig _photosConfig;
    private readonly UrlsConfig _urlsConfig;
    private readonly PredictionsGetter _predictionsGetter;

    public PredictionsService(ILogger<PredictionsService> logger,
        IInfoService infoService,
        IOptions<List<YoloRunnerConfig>> yoloRunnersConfig,
        IOptions<PhotosConfig> photosConfig,
        IOptions<UrlsConfig> urlsConfig)
    {
        _logger = logger;
        _infoService = infoService;
        _yoloRunnersConfig = yoloRunnersConfig.Value;
        ValidateYoloRunnersConfig(_yoloRunnersConfig);
        _photosConfig = photosConfig.Value;
        _urlsConfig = urlsConfig.Value;
        _predictionsGetter = new PredictionsGetter(logger, _yoloRunnersConfig);
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
    
    public async Task<List<YoloRunResultDto>> GetAllPredictionsAsync(IFormFile photo)
    {
        var modelsList = _infoService.GetAvailableYoloModelsAsync();
        ValidatePhoto(photo);
        
        return await GetPredictionsAsync(photo, modelsList);
    }
   
    public async Task<List<PredictionDtoBase>> GetPredictionsAllDetectedAsync(PredictionPropsDto propsDto)
    {
        var modelCount = GetModelsList(propsDto).Count;
        var threshold = propsDto.AgreeRatio * modelCount;

        return await GetMergedPredictions<PredictionDtoBase>(propsDto, threshold);
    }
    
    public async Task<List<PredictionWithCountDto>> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto)
    {
        return await GetMergedPredictions<PredictionWithCountDto>(propsDto, new object());
    }
    
    private async Task<List<TResult>> GetMergedPredictions<TResult>(PredictionPropsDto propsDto, object args)
    {
        var predictions = await GetPredictionsAsync(propsDto);

        var predictionsMerger = PredictionsMergerFactory.Create<TResult>();
        
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

    public async Task<List<YoloRunResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto)
    {
        var modelsList = GetModelsList(propsDto);
        
        ValidateProps(propsDto.Photo, modelsList);

        return await GetPredictionsAsync(propsDto.Photo, modelsList!);
    }

    private async Task<List<YoloRunResultDto>> GetPredictionsAsync(IFormFile photo, List<string> modelsList)
    {
        var hostedPhoto = await FileUtils
            .SaveAndHostFile(_photosConfig.Path, _urlsConfig.ContainerUrl, photo);

        var predictions = await _predictionsGetter
            .GetPredictions("https://centricconsulting.com/wp-content/uploads/2018/07/Group-Meeting.jpg", modelsList);

        FileUtils.DeleteFile(hostedPhoto.Path);

        return predictions;
    }

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
        var fileExtension = photo.FileName.Split(".").Last();
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

        return modelsListProps ?? _infoService.GetAvailableYoloModelsAsync();
    }
}