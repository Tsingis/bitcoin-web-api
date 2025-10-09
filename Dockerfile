FROM mcr.microsoft.com/dotnet/sdk:9.0.305-alpine3.22 AS build

WORKDIR /app

COPY src .
COPY Directory.Build.props .
COPY nuget.config .

RUN dotnet restore --locked-mode ./Api/Api.csproj

RUN dotnet publish ./Api/Api.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0.9-alpine3.22

COPY --from=build /app/publish .

# Use default non-root user
USER app

ENTRYPOINT ["dotnet", "Api.dll"]
