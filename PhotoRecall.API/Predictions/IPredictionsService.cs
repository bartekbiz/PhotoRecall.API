using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<YoloRunResultDto>> GetAllPredictionsAsync(IFormFile photo);
    Task<List<YoloRunResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto);
    Task<List<PredictionWithCountDto>> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto);
    Task<List<PredictionDtoBase>> GetPredictionsAllDetectedAsync(PredictionPropsDto propsDto);
}