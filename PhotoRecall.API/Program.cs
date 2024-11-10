using Data;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using PhotoRecall.API.Administration;
using PhotoRecall.API.Info;
using PhotoRecall.API.Middleware;
using PhotoRecall.API.Predictions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

#region Inject Services
// Configuration
#if !DEBUG
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
#endif

builder.Services.Configure<UrlsConfig>(builder.Configuration.GetSection("Urls"));
builder.Services.Configure<PhotosConfig>(builder.Configuration.GetSection("Photos"));
builder.Services.Configure<List<YoloRunnerConfig>>(builder.Configuration.GetSection("YoloRunners"));

var loggingConfig = new LoggingConfig();
builder.Services.AddSingleton(loggingConfig);
builder.Configuration.GetSection("Logging").Bind(loggingConfig);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(x =>
{
    x.IncludeScopes = true;

    x.IncludeFormattedMessage = true;
    
    x.AddOtlpExporter(a =>
    {
        a.Endpoint = new Uri(loggingConfig.Seq.Uri);
        a.Protocol = OtlpExportProtocol.HttpProtobuf;
        a.Headers = $"X-Seq-ApiKey={loggingConfig.Seq.ApiKey}";
    });
});

builder.Services.AddControllers();

// Middleware
builder.Services.AddScoped<InfoLoggingMiddleware>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Services
builder.Services.AddScoped<IPredictionsService, PredictionsService>();
builder.Services.AddScoped<IInfoService, InfoService>();
builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseMiddleware<InfoLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.UseStaticFiles();

app.Run();
