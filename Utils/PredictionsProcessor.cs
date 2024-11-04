using Data;
using Data.Dtos;

namespace Utils;

public class PredictionsProcessor
{
    private struct MergedPredictionPerYoloRunner
    {
        public YoloRunnerInfoDto YoloRunnerInfoDto { get; set; }
        public List<MergedPrediction> MergedPredictions { get; set; }
    }
    
    private struct MergedPrediction
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double Confidence { get; set; }
    }
    
    public List<MergedPredictionDto> MergeByVoting(List<YoloRunnerResultDto> predictions)
    {
        if (predictions.Count <= 0)
        {
            return [];
        }

        var mergedPerYoloRunner = new List<MergedPredictionPerYoloRunner>();
        
        foreach (var yoloRunnerResult in predictions)
        {
            if (yoloRunnerResult.Predictions == null)
                continue;

            var merged = yoloRunnerResult.Predictions
                .GroupBy(g => g.Name)
                .Select(group => new MergedPrediction
                {
                    Name = group.Key, 
                    Count = group.Count(), 
                    Confidence = group.Sum(s => s.Confidence) / (double)group.Count()
                })
                .ToList();

            mergedPerYoloRunner.Add(new MergedPredictionPerYoloRunner
            {
                YoloRunnerInfoDto = yoloRunnerResult.YoloRunnerInfo,
                MergedPredictions = merged
            });
        }

        return Vote(mergedPerYoloRunner);
    }

    private List<MergedPredictionDto> Vote(List<MergedPredictionPerYoloRunner> mergedPerYoloRunner)
    {
        var result = new List<MergedPredictionDto>();

        var selected = mergedPerYoloRunner
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Name);

        foreach (var group in selected)
        {
            result.Add(new MergedPredictionDto
            {
                Name = group.Key,
                Count = (int)Math.Round(group
                    .Select(s => (double)s.Count * s.Confidence).Sum() / mergedPerYoloRunner.Count)
            });
        }
        
        return result
            .Where(w => w.Count != 0)
            .ToList();
    }
}