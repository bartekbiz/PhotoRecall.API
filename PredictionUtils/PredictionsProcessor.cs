using Data;
using Data.Dtos;

namespace PredictionUtils;

public static class PredictionsProcessor
{
    public static List<PredictionDto> MergeByVoting(List<YoloRunnerResultDto> predictions)
    {
        var result = new List<PredictionDto>();
        
        if (predictions.Count <= 0)
        {
            return result;
        } 
        
        var firstPrediction = predictions.First();
        
        // foreach (var yoloRunnerName in predictions.Keys.Take(new Range(1, Index.End)))
        // {
        //     
        // }

        return result;
    }
}