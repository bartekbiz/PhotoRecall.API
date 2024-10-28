using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<PredictionDto>> GetVotedPredictionsAsync(HttpRequest request, IFormFile photo);
    Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(HttpRequest request, IFormFile photo);
}