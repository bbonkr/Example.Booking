using Example.Booking.Data;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using kr.bbon.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var defaultApiVersion = new ApiVersion(1, 0);
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
builder.Services.ConfigureAppOptions();

builder.Services.AddOptions<JsonSerializerOptions>()
    .Configure((options) =>
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        var migrationAssemblyName = typeof(Example.Booking.Data.SqlServer.Placeholder).Assembly.GetName().Name;
        sqlServerOptions.MigrationsAssembly(migrationAssemblyName);
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionHandlerFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApiVersioningAndSwaggerGen(defaultApiVersion);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwaggerUIWithApiVersioning();

//app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext?.Database.Migrate();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
