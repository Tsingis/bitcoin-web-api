[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Tsingis_bitcoin-web-api&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Tsingis_bitcoin-web-api) [![Deploy Status](https://github.com/tsingis/bitcoin-web-api/actions/workflows/deploy.yml/badge.svg)](https://github.com/Tsingis/bitcoin-web-api/actions/workflows/deploy.yml)

# About

API based on [GoinGecko's public API.](https://www.coingecko.com/en/api/documentation) to get initial data for bitcoin with euro as currency. API attempts to give answer to following questions with given date range.

1. How many days is the longest downward trend?
2. Which date has the highest trading volume?
3. What are the best days to buy and sell?

API documentation

-   Swagger [here](https://ca-bitcoin-web-api.salmonflower-f146d48d.northeurope.azurecontainerapps.io/swagger)
-   Scalar [here](https://ca-bitcoin-web-api.salmonflower-f146d48d.northeurope.azurecontainerapps.io/scalar)

Tools used:

-   .NET SDK
-   .NET report generator `dotnet tool install -g dotnet-reportgenerator-globaltool`
-   Docker
-   Azure
-   Terraform
-   WireMock

Docker

-   Run via Docker `docker compose up --build`

Api

-   Launch `dotnet run --launch-profile dev --project src/Api/Api.csproj`
