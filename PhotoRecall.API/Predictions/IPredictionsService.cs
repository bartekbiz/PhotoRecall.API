using Data;
using Data.Dtos;

namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<ModelRunResultDto>> GetAllPredictionsAsync(IFormFile photo);
    Task<List<ModelRunResultDto>> GetPredictionsAsync(PredictionPropsDto propsDto);
    Task<List<PredictionDtoMerged>> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto);
    Task<List<PredictionDtoMerged>> GetPredictionsAllDetectedAsync(PredictionPropsDto propsDto);
}