using AuctionSystem.Application;
using AuctionSystem.Infrastructure;
using AuctionSystem.Presentation;
using AuctionSystem.Presentation.Extensions;
using AuctionSystem.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
