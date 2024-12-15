namespace Data.Configuration;

public class ApiConfig
{
    public SynonymsConfig Synonyms { get; set; }
}

public class SynonymsConfig
{
    public string Uri { get; set; }
    public string Key { get; set; }
}