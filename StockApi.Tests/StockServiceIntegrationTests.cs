using Microsoft.Extensions.Logging;
using Moq;
using StockApi.Repositories;
using StockApi.Services;

namespace StockApi.Tests
{
    public class StockServiceIntegrationTests
    {
        private readonly IStockRepository _repo;
        private readonly IStockService _service;

        public StockServiceIntegrationTests()
        {
            // For the integration tests the Json repo is used,
            // this ensures we are testing the real data from the stock.json
            var loggerRepo = new Mock<ILogger<JsonStockRepository>>();
            var loggerService = new Mock<ILogger<StockService>>();
            var repo = new JsonStockRepository(loggerRepo.Object);
            repo.LoadFromJonPath("stocks.json");
            _repo = repo;
            _service = new StockService(loggerService.Object, _repo);
        }

        [Fact]
        public async Task GetAllTickersAsync_ReturnsAllTickers()
        {
            // Act
            var tickers = await _service.GetAllTickersAsync();

            // Assert
            Assert.Contains("AAPL", tickers);
            Assert.Contains("MSFT", tickers);
            Assert.Contains("GOOGL", tickers);
        }

        [Fact]
        public async Task GetStockByTickerAsync_ReturnsCorrectStock()
        {
            // Act
            var stock = await _service.GetStockByTickerAsync("AAPL");

            // Assert
            Assert.NotNull(stock);
            Assert.Equal("AAPL", stock.Ticker);
            Assert.Equal(198.15m, stock.Open);
            Assert.Equal(202.30m, stock.Close);
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ReturnsCorrectShares()
        {
            // Arrange
            decimal budget = 1000m;
            var expectedShares = (int)Math.Floor(budget / 202.30m); // Based on AAPL close price in stocks.json

            // Act
            var option = await _service.GetBuyingOptionAsync("AAPL", budget);

            // Assert
            Assert.NotNull(option);
            Assert.Equal("AAPL", option.Ticker);
            Assert.Equal(budget, option.Budget);
            Assert.Equal(expectedShares, option.Shares);
        }
    }
}