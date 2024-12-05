using Data;
using Data.Dtos;

namespace Utils.PredictionsMergers;

public class PredictionsMergerAllDetected : PredictionsMerger<PredictionDtoBase>
{
    private readonly List<MergedPerModel> _mergedPerModel = [];

    #region Structs

    private struct MergedPerModel
    {
        public YoloRunInfoDto YoloRunInfoDto { get; set; }
        public List<PredictionDtoBase> MergedPredictions { get; set; }
    }

    #endregion

    #region Merge

    public override List<PredictionDtoBase> Merge(List<YoloRunResultDto> predictions)
    {
        base.Merge(predictions);
        
        if (Predictions.Count <= 0)
        {
            return [];
        }
        
        MergePerModel();

        return _mergedPerModel
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Class)
            .Select(s => s.First())
            .ToList();
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
                    
                    return new PredictionDtoBase
                    {
                        Class = group.Key,
                        Name = firstItem != null ? firstItem.Name : string.Empty,
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

    #endregion
}