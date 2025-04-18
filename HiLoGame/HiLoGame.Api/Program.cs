using Application.Registry;
using Infrastructure.EventStorage.InMemory.Registry;
using HiLoGame.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCommandHandlers();
builder.Services.AddProviders();
builder.Services.AddInMemoryEventStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHiLoGameEndpoints();

app.Run();
