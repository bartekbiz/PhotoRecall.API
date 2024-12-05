using Data;
using Data.Dtos;

namespace Utils.PredictionsMergers;

public class PredictionsMergerWithCounts : PredictionsMerger<PredictionWithCountDto>
{
    private readonly List<MergedPerModel> _mergedPerModel = [];

    #region Structs

    private struct MergedPerModel
    {
        public YoloRunInfoDto YoloRunInfoDto { get; set; }
        public List<PredictionWithCountDto> MergedPredictions { get; set; }
    }

    #endregion

    #region Merge

    public override List<PredictionWithCountDto> Merge(List<YoloRunResultDto> predictions, object args)
    {
        base.Merge(predictions, args);
        
        if (Predictions.Count <= 0)
        {
            return [];
        }
        
        MergePerModel();
        
        return Vote();
    }

    private void MergePerModel()
    {
        foreach (var yoloRunResult in Predictions)
        {
            if (yoloRunResult.Predictions == null)
                continue;

            var merged = yoloRunResult.Predictions
                .GroupBy(g => g.Class)
                .Select(group =>
                {
                    var firstItem = group.FirstOrDefault();
                    
                    return new PredictionWithCountDto
                    {
                        Class = group.Key,
                        Name = firstItem != null ? firstItem.Name : string.Empty,
                        Count = group.Count()
                    };
                })
                .ToList();

            _mergedPerModel.Add(new MergedPerModel
            {
                YoloRunInfoDto = yoloRunResult.YoloRunInfo,
                MergedPredictions = merged
            });
        }
    }
    
    #region Vote

    private List<PredictionWithCountDto> Vote()
    {
        var groupsByClass = _mergedPerModel
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Class);

        var result = groupsByClass
            .Select(group =>
            {
                var firstItem = group.First();
                firstItem.Count = VoteOnClassCount(group, _mergedPerModel.Count);
                return firstItem;
            }).ToList();

        return result
            .Where(w => w.Count != 0)
            .ToList();
    }

    private static int VoteOnClassCount(IGrouping<int, PredictionWithCountDto> group, int modelCount)
    {
        return (int)Math.Round((double)group
            .Select(s => s.Count)
            .Sum() / modelCount);
    }

    #endregion
    
    #endregion
}