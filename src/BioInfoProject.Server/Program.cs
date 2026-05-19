using BioInfoProject.Server.Data;
using BioInfoProject.Server.Services;
using BioInfoProject.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddDbContext<BioInfoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BioInfoDatabase")));
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
