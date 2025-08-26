using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockApi.Controllers.v1;
using StockApi.Models;
using StockApi.Services;

namespace StockApi.Tests
{
    public class StockControllerTests
    {
        private readonly Mock<IStockService> _stockServiceMock;
        private readonly Mock<ILogger<StockController>> _loggerMock;
        private readonly StockController _controller;

        public StockControllerTests()
        {
            _stockServiceMock = new Mock<IStockService>();
            _loggerMock = new Mock<ILogger<StockController>>();
            _controller = new StockController(_loggerMock.Object, _stockServiceMock.Object);
        }

        [Fact]
        public async Task GetAllTickers_ShouldReturnListOfTickers()
        {
            // Arrange
            var expectedTickers = new List<string> { "AAPL", "MSFT", "GOOGL" };
            _stockServiceMock.Setup(s => s.GetAllTickersAsync()).ReturnsAsync(expectedTickers.AsEnumerable());

            // Act
            var result = await _controller.GetAllTickers() as OkObjectResult;

            // Assert
            var tickers = Assert.IsType<List<string>>(result?.Value);
            Assert.Contains("AAPL", tickers);
            Assert.Contains("MSFT", tickers);
            Assert.Contains("GOOGL", tickers);
            Assert.Equal(3, tickers.Count);
        }

        [Fact]
        public async Task GetTickerDetails_ShouldReturnCorrectDetailsForAAPLAsync()
        {
            // Arrange
            var mockResponse = new Stock
            {
                Ticker = "AAPL",
                Date = new DateOnly(2025, 7, 17),
                Open = 198.15m,
                Close = 202.30m,
                High = 203.00m,
                Low = 197.80m,
                Volume = 24000000
            };
            _stockServiceMock.Setup(s => s.GetStockByTickerAsync("AAPL"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetTickerDetails("AAPL") as OkObjectResult;

            // Assert
            dynamic? stock = result?.Value;
            Assert.NotNull(stock);
            Assert.Equal("AAPL", (string?)stock?.Ticker);
            Assert.Equal(198.15m, (decimal?)stock?.Open);
            Assert.Equal(202.30m, (decimal?)stock?.Close);
        }

        [Fact]
        public async Task GetBuyingOption_ShouldReturnNumberOfSharesForBudget()
        {
            // Arrange
            var mockResponse = new BuyingOption
            {
                Ticker = "AAPL",
                Budget = 1000m,
                Shares = 4 // 1000 / 202.30 ~= 4
            };
            _stockServiceMock.Setup(s => s.GetBuyingOptionAsync("AAPL", 1000m))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetBuyingOption("AAPL", 1000) as OkObjectResult;

            // Assert
            dynamic? response = result?.Value;
            Assert.NotNull(response);
            Assert.Equal("AAPL", (string?)response?.Ticker);
            Assert.Equal(1000, (decimal?)response?.Budget);
            Assert.Equal(4, (int?)response?.Shares); // 1000 / 202.30 ≈ 4
        }
    }
}