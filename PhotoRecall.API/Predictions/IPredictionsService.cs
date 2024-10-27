using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<PredictionDto>> GetVotedPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig);
    Task<List<YoloRunnerResultDto>> GetAllPredictionsAsync(List<YoloRunnerConfig> yoloRunnersConfig);
}