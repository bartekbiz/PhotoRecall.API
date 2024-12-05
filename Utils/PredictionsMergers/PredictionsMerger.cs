using Data.Dtos;

namespace Utils.PredictionsMergers;

public interface IPredictionsMerger<TResult>
{
    List<TResult> Merge(List<YoloRunResultDto> predictions);
}

public abstract class PredictionsMerger<TResult> : IPredictionsMerger<TResult>
{
    protected List<YoloRunResultDto> Predictions = [];
    
    public virtual List<TResult> Merge(List<YoloRunResultDto> predictions)
    {
        Predictions = predictions;

        return [];
    }
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
