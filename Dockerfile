FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY *.slnx .
COPY src/AuctionSystem.Api/*.csproj ./src/AuctionSystem.Api/
COPY src/AuctionSystem.Application/*.csproj ./src/AuctionSystem.Application/
COPY src/AuctionSystem.Domain/*.csproj ./src/AuctionSystem.Domain/
COPY src/AuctionSystem.Infrastructure/*.csproj ./src/AuctionSystem.Infrastructure/
COPY src/AuctionSystem.Presentation/*.csproj ./src/AuctionSystem.Presentation/

RUN dotnet restore src/AuctionSystem.Api/AuctionSystem.Api.csproj

COPY src/ ./src/
WORKDIR ./src/AuctionSystem.Api
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./

RUN apt-get update && apt-get install -y curl

ENTRYPOINT ["dotnet", "AuctionSystem.Api.dll"]
