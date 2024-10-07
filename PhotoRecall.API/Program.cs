using PhotoRecall.API.Middleware;
using PhotoRecall.API.Predictions;

var builder = WebApplication.CreateBuilder(args);

#region Inject Services
builder.Services.AddControllers();

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Services
builder.Services.AddScoped<IPredictionsService, PredictionsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
