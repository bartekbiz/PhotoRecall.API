namespace PhotoRecall.API.Predictions;

public interface IPredictionsService
{
    Task<List<PredictionDto>> GetPredictionsAsync();
}