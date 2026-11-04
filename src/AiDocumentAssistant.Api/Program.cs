
using AiDocumentAssistant.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();

// Register the service used by DocumentTextController.
builder.Services.AddScoped<DocumentTextService>();
builder.Services.AddScoped<DocumentEvidenceService>();
builder.Services.AddScoped<CosineSimilarityService>();
builder.Services.AddScoped<DocumentChunkingService>();
builder.Services.AddScoped<DocumentSemanticSearchService>();
builder.Services.AddHttpClient<DocumentSummaryService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});


builder.Services.AddHttpClient<DocumentQuestionService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddHttpClient<DocumentEmbeddingService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
