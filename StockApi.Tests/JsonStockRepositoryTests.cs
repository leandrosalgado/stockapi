using Microsoft.Extensions.Logging;
using Moq;
using StockApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace StockApi.Tests
{
    public class JsonStockRepositoryTests
    {
        private readonly Mock<ILogger<JsonStockRepository>> _loggerMock;

        public JsonStockRepositoryTests()
        {
            _loggerMock = new Mock<ILogger<JsonStockRepository>>();
        }

        [Fact]
        public void LoadFromJonPath_ThrowsArgumentNullException_OnEmptyPath()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            Assert.Throws<ArgumentNullException>(() => repo.LoadFromJonPath(""));
        }

        [Fact]
        public void LoadFromJonPath_ThrowsArgumentException_OnInvalidChars()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);

            // To ensure the tests can run in difference OSs we get the invalid chars from the Path api
            // and use the first one to create an invalid path
            var invalidPathChars = Path.GetInvalidPathChars();
            Assert.Throws<ArgumentException>(() => repo.LoadFromJonPath($"invalid{invalidPathChars[0]}path.json"));
        }

        [Fact]
        public void LoadFromJonPath_ThrowsFileNotFoundException_OnNonExistingFile()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            Assert.Throws<FileNotFoundException>(() => repo.LoadFromJonPath("nonexistent.json"));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTickers()
        {
            // Prepare repo
            var repo = new JsonStockRepository(_loggerMock.Object);
            repo.LoadFromJonPath("stocks.json");

            // Act & Assert
            var result = await repo.GetAllAsync();
            Assert.Contains("AAPL", result);
            Assert.Contains("MSFT", result);
        }

        [Fact]
        public async Task GetByTickerAsync_ReturnsStock_WhenExists()
        {
            // Prepare repo
            var repo = new JsonStockRepository(_loggerMock.Object);
            repo.LoadFromJonPath("stocks.json");

            var result = await repo.GetByTickerAsync("AAPL");
            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Ticker);
        }

        [Fact]
        public async Task GetByTickerAsync_ReturnsNull_WhenNotExists()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            repo.LoadFromJonPath("stocks.json");

            var result = await repo.GetByTickerAsync("TEST123");
            Assert.Null(result);
        }

        [Fact]
        public async Task LoadFromJonPath_IgnoresInvalidEntries_WhenIgnoreInvalidIsTrue()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            repo.LoadFromJonPath("Test-Files\\test-invalid-stock.json", true);

            // Only valid stocks should be loaded; the test file has only one valid entry
            var result = await repo.GetAllAsync();
            Assert.NotEmpty(result);
        }

        [Fact]
        public void LoadFromJonPath_ThrowsValidationException_WhenIgnoreInvalidIsFalse()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            Assert.Throws<ValidationException>(() => repo.LoadFromJonPath("Test-Files\\test-invalid-stock.json", false));
        }

        [Fact]
        public void LoadFromJonPath_InvalidJson()
        {
            var repo = new JsonStockRepository(_loggerMock.Object);
            Assert.Throws<System.Text.Json.JsonException>(() => repo.LoadFromJonPath("Test-Files\\invalid-json.json", true));
        }
    }
}