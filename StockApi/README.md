# StockApi

A simple .NET 8 Web API for viewing stock information and buying options.

## Features

- View all available stock tickers
- Get details for a specific stock ticker
- Get buying options of a stock given a budget
- Loads data from `stocks.json`
- API versioning

## Endpoints

### Get all tickers
```
GET /api/v1/stock/tickers
```
Response:
`200 OK`
```
["AAPL", "MSFT", "GOOGL", ...]
```

### Get details for a ticker
```
GET /api/v1/stock/{ticker}
```
Response:
`200 OK`
```
{
  "ticker": "AAPL",
  "open": 198.15,
  "close": 202.30,
  ...
}
```
Errors:
- `400 Bad Request` if ticker is empty
- `404 Not Found` if ticker does not exist

### Simulate buying shares
```
GET /api/v1/stock/{ticker}/buy?budget={amount}
```
Response:
`200 OK`
```
{
  "ticker": "AAPL",
  "budget": 1000,
  "shares": 4
}
```
Errors:
- `404 Not Found` if ticker does not exist or cannot be bought

## Running the API

1. Ensure `.NET 8 SDK` is installed.
2. Place a valid `stocks.json` file in the project root.
3. Run the API:
```
dotnet run --project StockApi
```
4. Access Swagger UI at `http://localhost:{port}/swagger` (development only).

