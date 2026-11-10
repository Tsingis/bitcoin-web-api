FROM mcr.microsoft.com/dotnet/sdk:11.0.100-alpine3.24 AS build

WORKDIR /app

COPY src .
COPY Directory.Build.props .
COPY nuget.config .

RUN dotnet restore --locked-mode ./Api/Api.csproj

RUN dotnet publish ./Api/Api.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:11.0.0-azurelinux3.0-distroless

COPY --from=build /app/publish .

# Use default non-root user
USER app

ENTRYPOINT ["dotnet", "Api.dll"]
