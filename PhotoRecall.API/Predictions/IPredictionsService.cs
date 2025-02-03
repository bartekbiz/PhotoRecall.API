using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<ModelRunResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto);
    Task<List<PredictionDtoMerged>> GetMergedPredictionsAsync(PredictionVotingPropsDto propsDto);
    Task<List<PredictionDtoMerged>> GetMergedPredictionsWithCountsAsync(PredictionPropsDto propsDto);
}