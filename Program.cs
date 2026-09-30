using System.Text.Json.Serialization;
using Booking.API.Endpoints;
using Booking.Application.State;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.RespectNullableAnnotations = true;
  options.SerializerOptions.RespectRequiredConstructorParameters = true;

  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IGlobalStateHandler, GlobalStateHandler>();

var app = builder.Build();

var api = app.MapGroup("/api");
api.MapBookingsEndpoints();
api.MapResourceEndpoints();
api.MapUserEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();
