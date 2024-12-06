using Data.Dtos;

namespace Utils.PredictionsMergers;

public interface IPredictionsMerger<TResult>
{
    List<TResult> Merge(List<YoloRunResultDto> predictions, object args);
}

public abstract class PredictionsMerger<TResult> : IPredictionsMerger<TResult>
{
    protected List<YoloRunResultDto> Predictions = [];
    protected List<MergedForModel> MergedPerModel = [];
    
    public virtual List<TResult> Merge(List<YoloRunResultDto> predictions, object args)
    {
        Predictions = predictions;

        return [];
    }
    
    protected void MergePerModel(Func<List<PredictionDto>, List<TResult>> mergeDelegate)
    {
        foreach (var yoloRunResult in Predictions)
        {
            if (yoloRunResult.Predictions == null)
                continue;

            var merged = mergeDelegate(yoloRunResult.Predictions);

            MergedPerModel.Add(new MergedForModel
            {
                YoloRunInfoDto = yoloRunResult.YoloRunInfo,
                MergedPredictions = merged
            });
        }
    }
    
    #region Structs

    protected struct MergedForModel
    {
        public YoloRunInfoDto YoloRunInfoDto { get; set; }
        public List<TResult> MergedPredictions { get; set; }
    }

    #endregion
} 

public static class PredictionsMergerFactory
{
    public static IPredictionsMerger<TResult>? Create<TResult>()
    {
        if (typeof(TResult) == typeof(PredictionDtoBase))
        {
            return (IPredictionsMerger<TResult>) new PredictionsMergerAllDetected();
        }
        
        if (typeof(TResult) == typeof(PredictionWithCountDto))
        {
            return (IPredictionsMerger<TResult>) new PredictionsMergerWithCounts();
        }
        
        return null;
    }
}
