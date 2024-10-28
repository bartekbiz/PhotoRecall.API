using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<PredictionDto>> GetVotedPredictionsAsync();
    Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync();
}