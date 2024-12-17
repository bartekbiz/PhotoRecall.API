using Data;
using Data.Configuration;
using Data.Enums;

namespace Utils;

public interface IClassSearcher<TResult> where TResult : Enum
{
    public Task<List<TResult>> Search(string input);
}

public abstract class ClassSearcher<TResult> : IClassSearcher<TResult>
    where TResult : Enum
{
    protected SynonymsConfig SynonymsConfig;

    public ClassSearcher(SynonymsConfig synonymsConfig)
    {
        SynonymsConfig = synonymsConfig;
    }

    public virtual Task<List<TResult>> Search(string input)
    {
        throw new NotImplementedException();
    }
}