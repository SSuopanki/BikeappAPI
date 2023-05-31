using BikeappAPI.Models;
using BikeappAPI.Repositories;
using BikeappAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

var AllowOrigins = "AllowOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5173").AllowAnyHeader().AllowAnyMethod();
                      });
});

// Retrieve the connection string from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = configuration.GetConnectionString("Bikeapp");

builder.Services.AddDbContext<BikeappContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<JourneysRepository>();
builder.Services.AddScoped<StationsRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
