using BikeappAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var AllowOrigins = "AllowOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5173");
                      });
});
// Add services to the container.

builder.Services.AddDbContext<BikeappContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Bikeapp")));

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
