namespace Data;

public class LoggingConfig
{
    public SeqConfig Seq { get; set; }
}

public class SeqConfig
{
    public string Uri { get; set; }
    public string ApiKey { get; set; }
}