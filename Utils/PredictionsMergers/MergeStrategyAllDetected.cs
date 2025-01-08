using Data;
using Data.Dtos;

namespace Utils.PredictionsMergers;

public class MergeStrategyAllDetected : MergeStrategyBase
{
    private double _threshold;
    
    public override List<PredictionDtoMerged> Merge(List<ModelRunResultDto> predictions, object args)
    {
        base.Merge(predictions, args);

        _threshold = (double)args;
        
        if (Predictions.Count <= 0)
        {
            return [];
        }

        MergePerModel(MergeDelegate);

        return Vote();
    }

    private List<PredictionDtoMerged> MergeDelegate(List<PredictionDto> predictions)
    {
        return predictions
            .GroupBy(g => g.Class)
            .Select(group =>
            {
                var firstItem = group.FirstOrDefault();
                    
                return new PredictionDtoMerged
                {
                    Class = group.Key,
                    Name = firstItem != null ? firstItem.Name : string.Empty,
                };
            })
            .ToList();
    }

    private List<PredictionDtoMerged> Vote()
    {
        return MergedPerModel
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Class)
            .Where(g => g.Count() >= _threshold)
            .Select(s => s.First())
            .ToList();
    }
}