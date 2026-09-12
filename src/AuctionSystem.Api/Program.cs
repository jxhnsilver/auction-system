using AuctionSystem.Api.Extensions;
using AuctionSystem.Application;
using AuctionSystem.Infrastructure;
using AuctionSystem.Presentation;
using AuctionSystem.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks(builder.Configuration);

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseSwaggerDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
