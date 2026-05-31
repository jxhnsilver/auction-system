using AuctionSystem.Application;
using AuctionSystem.Infrastructure;
using AuctionSystem.Presentation;
using AuctionSystem.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
