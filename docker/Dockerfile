FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VaultHistory.User.slnx ./
COPY src ./src
COPY tests ./tests

RUN dotnet restore VaultHistory.User.slnx

RUN dotnet publish src/VaultHistory.User.Api/VaultHistory.User.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "VaultHistory.User.Api.dll"]
