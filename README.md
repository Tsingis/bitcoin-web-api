# About

API based on [GoinGecko's public API.](https://www.coingecko.com/en/api/documentation) to get initial data for bitcoin with euro as currency. API attempts to give answer to following questions with given date range.

1. How many days is the longest downward trend?
2. Which date has the highest trading volume?
3. What are the best days to buy and sell?

Use API hosted on [Render](https://bitcoin-web-api.onrender.com) with built-in Swagger
or with any suitable tool of your choice.

Input date format: `yyyy-MM-dd`

Endpoints:

- /longestdownwardtrend
- /highestradingvolume
- /buyandsell

## Development

Requirements:

- [.NET 8 SDK ](https://dotnet.microsoft.com/download/dotnet/8.0) installed

Run API locally `dotnet watch run --project App`

Run API locally via Docker `docker-compose up app --build`

Run tests `dotnet test`
