using Data;
using Data.Dtos;

namespace Utils.PredictionsMergers;

public class PredictionsMergerWithCounts : PredictionsMerger<PredictionWithCountDto>
{
    private readonly List<YoloRunnerResultDto> _predictions;
    private readonly List<MergedPerYoloRunner> _mergedPerYoloRunner = [];

    public PredictionsMergerWithCounts(List<YoloRunnerResultDto> predictions)
    {
        _predictions = predictions;
    }

    #region Structs

    private struct MergedPerYoloRunner
    {
        public YoloRunnerInfoDto YoloRunnerInfoDto { get; set; }
        public List<PredictionWithCountDto> MergedPredictions { get; set; }
    }

    #endregion

    #region Merge

    public override List<PredictionWithCountDto> Merge()
    {
        if (_predictions.Count <= 0)
        {
            return [];
        }
        
        MergePerYoloRunner();
        
        return Vote();
    }

    private void MergePerYoloRunner()
    {
        foreach (var yoloRunnerResult in _predictions)
        {
            if (yoloRunnerResult.Predictions == null)
                continue;

            var merged = yoloRunnerResult.Predictions
                .GroupBy(g => g.Name)
                .Select(group => new PredictionWithCountDto
                {
                    Name = group.Key, 
                    Count = group.Count()
                })
                .ToList();

            _mergedPerYoloRunner.Add(new MergedPerYoloRunner
            {
                YoloRunnerInfoDto = yoloRunnerResult.YoloRunnerInfo,
                MergedPredictions = merged
            });
        }
    }

    #endregion
    
    #region Vote

    private List<PredictionWithCountDto> Vote()
    {
        var groupsByName = _mergedPerYoloRunner
            .SelectMany(s => s.MergedPredictions)
            .GroupBy(g => g.Name);

        var result = groupsByName
            .Select(group => 
                new PredictionWithCountDto
                {
                    Name = group.Key, 
                    Count = VoteOnClassCount(group, _mergedPerYoloRunner.Count)
                }).ToList();

        return result
            .Where(w => w.Count != 0)
            .ToList();
    }

    private static int VoteOnClassCount(IGrouping<string, PredictionWithCountDto> group, int yoloRunnerCount)
    {
        return (int)Math.Round((double)group
            .Select(s => s.Count)
            .Sum() / yoloRunnerCount);
    }

    #endregion
}