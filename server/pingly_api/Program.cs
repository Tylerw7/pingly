using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using pingly_api.Config;
using pingly_api.services;
using System.Threading.Channels;
using pingly_api.Data;


var config = AppConfig.Load();

// var builder = WebApplication.CreateBuilder(args);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton(config);
builder.Services.AddSingleton(Channel.CreateUnbounded<string>());

// Services
builder.Services.AddScoped<TopicService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(config.DatabaseConnectionString));


var app = builder.Build();



// test for channels
var channel = app.Services.GetRequiredService<Channel<string>>();
_ = Task.Run(async () =>
{
    await foreach (var message in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine($"Recieved: {message}");

        await Task.Delay(2000);

        Console.WriteLine($"Finished: {message}");
    }
});





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();



app.MapGet("/", () =>
{
    return new { message = "success the server is running! And the database is created" };
});



app.Run();


