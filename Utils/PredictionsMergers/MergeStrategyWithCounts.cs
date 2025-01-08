using Data;
using Data.Dtos;

namespace Utils.PredictionsMergers;

public class MergeStrategyWithCounts : MergeStrategyBase
{
    public override List<PredictionDtoMerged> Merge(List<ModelRunResultDto> predictions, object args)
    {
        base.Merge(predictions, args);
        
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
                    Count = group.Count()
                };
            })
            .ToList();
    }

    private List<PredictionDtoMerged> Vote()
    {
        var groupsByClass = MergedPerModel
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Class);

        var result = groupsByClass
            .Select(group =>
            {
                var firstItem = group.First();
                firstItem.Count = VoteOnClassCount(group, MergedPerModel.Count);
                return firstItem;
            }).ToList();

        return result
            .Where(w => w.Count != 0)
            .ToList();
    }

    private static int VoteOnClassCount(IGrouping<int, PredictionDtoMerged> group, int modelCount)
    {
        return (int)Math.Round((double)group
            .Select(s => s.Count)
            .Sum() / modelCount);
    }
}