# StockApi.Tests

Integration and unit tests for the StockApi service layer and controllers.

## Features

- Tests for retrieving all tickers
- Tests for getting stock details by ticker
- Tests for simulating stock purchases
- Error handling and edge case coverage

## Running Tests

1. Ensure `.NET 8 SDK` is installed.
2. Run tests:
```
dotnet test StockApi.Tests
```

## Test Structure

- Uses xUnit for test framework
- Mocks logging dependencies
- Loads real data from `stocks.json` for integration tests

## Example Test Cases

- `GetAllTickersAsync_ReturnsAllTickers`: Verifies all tickers are returned.
- `GetStockByTickerAsync_ReturnsCorrectStock`: Verifies correct stock details are returned.
- `GetBuyingOptionAsync_ReturnsCorrectShares`: Verifies share calculation for a given budget.

