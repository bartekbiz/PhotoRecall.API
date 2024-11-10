using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(IFormFile photo);
    Task<List<YoloRunnerResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto);
    Task<List<PredictionWithCountDto>> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto);
}