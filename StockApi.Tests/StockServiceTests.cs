using Microsoft.Extensions.Logging;
using Moq;
using StockApi.Models;
using StockApi.Repositories;
using StockApi.Services;

namespace StockApi.Tests
{
    public class StockServiceTests
    {
        private readonly Mock<IStockRepository> _repoMock;
        private readonly Mock<ILogger<StockService>> _loggerMock;
        private readonly StockService _service;

        public StockServiceTests()
        {
            _repoMock = new Mock<IStockRepository>();
            _loggerMock = new Mock<ILogger<StockService>>();
            _service = new StockService(_loggerMock.Object, _repoMock.Object);
        }

        [Fact]
        public async Task GetAllTickersAsync_ReturnsTickers()
        {
            var stocks = new List<string> { "AAPL", "MSFT", "GOOGL" };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stocks);

            var result = await _service.GetAllTickersAsync();

            Assert.Contains("AAPL", result);
            Assert.Contains("MSFT", result);
            Assert.Contains("GOOGL", result);
        }

        [Fact]
        public async Task GetStockByTickerAsync_ReturnsStock_WhenExists()
        {
            var stock = new Stock { Ticker = "AAPL", Close = 202.30m };
            _repoMock.Setup(r => r.GetByTickerAsync("AAPL")).ReturnsAsync(stock);

            var result = await _service.GetStockByTickerAsync("AAPL");

            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Ticker);
            Assert.Equal(202.30m, result.Close);
        }

        [Fact]
        public async Task GetStockByTickerAsync_ReturnsNull_WhenNotExists()
        {
            _repoMock.Setup(r => r.GetByTickerAsync("TEST123")).ReturnsAsync((Stock?)null);

            var result = await _service.GetStockByTickerAsync("TEST123");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ReturnsCorrectShares()
        {
            var stock = new Stock { Ticker = "AAPL", Close = 202.30m };
            _repoMock.Setup(r => r.GetByTickerAsync("AAPL")).ReturnsAsync(stock);

            var result = await _service.GetBuyingOptionAsync("AAPL", 1000m);

            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Ticker);
            Assert.Equal(1000m, result.Budget);
            Assert.Equal(4, result.Shares); // 1000 / 202.30 ~= 4
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ReturnsNull_WhenStockNotFound()
        {
            _repoMock.Setup(r => r.GetByTickerAsync("TEST123")).ReturnsAsync((Stock?)null);

            var result = await _service.GetBuyingOptionAsync("TEST123", 1000m);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ThrowsException_WhenBudgetIsZeroOrNegative()
        {
            var stock = new Stock { Ticker = "AAPL", Close = 202.30m };
            _repoMock.Setup(r => r.GetByTickerAsync("AAPL")).ReturnsAsync(stock);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetBuyingOptionAsync("AAPL", 0m));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetBuyingOptionAsync("AAPL", -100m));
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ThrowsException_WhenTickerIsNullOrEmpty()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetBuyingOptionAsync(null!, 1000m));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetBuyingOptionAsync("", 1000m));
        }

        [Fact]
        public async Task GetBuyingOptionAsync_ReturnsZeroShares_WhenCloseIsZero()
        {
            var stock = new Stock { Ticker = "AAPL", Close = 0m };
            _repoMock.Setup(r => r.GetByTickerAsync("AAPL")).ReturnsAsync(stock);

            var result = await _service.GetBuyingOptionAsync("AAPL", 1000m);

            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Ticker);
            Assert.Equal(1000m, result.Budget);
            Assert.Equal(0, result.Shares);
        }
    }
}