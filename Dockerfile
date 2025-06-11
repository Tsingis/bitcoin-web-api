FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build

WORKDIR /app

COPY App .
COPY Directory.Build.props .
COPY NuGet.config .

RUN dotnet restore --locked-mode ./Api/Api.csproj

RUN dotnet publish ./Api/Api.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine

COPY --from=build /app/publish .

# Use default non-root user
USER app

ENTRYPOINT ["dotnet", "Api.dll"]
