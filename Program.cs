using server.Models;
using server.Services;
using server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<LogDatabaseSettings>(
    builder.Configuration.GetSection("LogDatabase"));

builder.Services.AddSignalR();

builder.Services.AddCors(
  o => o.AddPolicy("CorsPolicy", builder =>
  {
      builder
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials()
      .WithOrigins("http://localhost:4200");
  })
);

builder.Services.AddSingleton<LogService>();

builder.Services.AddControllers();
builder.Services.AddSingleton<TimerManager>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapPost("/close", (Log log) =>
{
    Console.WriteLine(log);
    return "ok";
});

app.MapHub<Loghub>("/loghub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
