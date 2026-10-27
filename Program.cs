using Microsoft.EntityFrameworkCore;
using MyFirstApi_cs.Data;
using MyFirstApi_cs.Router;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Products")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await database.Database.EnsureCreatedAsync();
}

app.MapApiRoutes();

app.Run();
