using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using pingly_api.Config;
using pingly_api.Data;
using pingly_api.Endpoints;


var config = AppConfig.Load();

// var builder = WebApplication.CreateBuilder(args);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton(config);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(config.DatabaseConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/", () =>
{
    return new { message = "success the server is running! And the database is created" };
});

app.MapTopicEndpoints();

app.Run();


