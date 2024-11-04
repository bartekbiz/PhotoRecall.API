using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<MergedPredictionDto>> GetVotedPredictionsAsync(IFormFile photo);
    Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(IFormFile photo);
}