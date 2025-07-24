using AlreadyMedia.Configs;
using AlreadyMedia.Contexts;
using AlreadyMedia.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>();

builder.Services
    .AddOptions<NasaDatasetConfig>()
    .Bind(builder.Configuration.GetSection("NasaDataset"));

builder.Services.AddNasaServices();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();