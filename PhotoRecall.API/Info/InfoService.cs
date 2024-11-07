using Data;
using Microsoft.Extensions.Options;

namespace PhotoRecall.API.Info;

public class InfoService(IOptions<List<YoloRunnerConfig>> yoloRunnersConfig) : IInfoService
{
    private readonly List<YoloRunnerConfig> _yoloRunnersConfig = yoloRunnersConfig.Value;

    public List<string> GetAvailableYoloModelsAsync()
    {
        return _yoloRunnersConfig.SelectMany(s => s.Models).ToList();
    }
}