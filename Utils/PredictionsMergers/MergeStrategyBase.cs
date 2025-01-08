using Data.Dtos;

namespace Utils.PredictionsMergers;

public abstract class MergeStrategyBase : IMergeStrategy
{
    protected List<ModelRunResultDto> Predictions = [];
    protected List<MergedForModel> MergedPerModel = [];
    
    public virtual List<PredictionDtoMerged> Merge(List<ModelRunResultDto> predictions, object args)
    {
        Predictions = predictions;

        return [];
    }
    
    protected void MergePerModel(Func<List<PredictionDto>, List<PredictionDtoMerged>> mergeDelegate)
    {
        foreach (var yoloRunResult in Predictions)
        {
            if (yoloRunResult.Predictions == null)
                continue;

            var merged = mergeDelegate(yoloRunResult.Predictions);

            MergedPerModel.Add(new MergedForModel
            {
                ModelRunInfoDto = yoloRunResult.ModelRunInfo,
                MergedPredictions = merged
            });
        }
    }
    
    #region Structs

    protected struct MergedForModel
    {
        public ModelRunInfoDto ModelRunInfoDto { get; set; }
        public List<PredictionDtoMerged> MergedPredictions { get; set; }
    }

    #endregion
}