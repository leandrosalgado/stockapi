using StockApi.Models;
using StockApi.Repositories;

namespace StockApi.Services
{
    /// <summary>
    /// Service that provides basic stock operations by interacting with the stock repository.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly ILogger<StockService> _logger;
        private readonly IStockRepository _repository;
        public StockService(ILogger<StockService> logger, IStockRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public Task<IEnumerable<string>> GetAllTickersAsync()
        {
            return _repository.GetAllAsync();
        }


        public async Task<Stock?> GetStockByTickerAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                _logger?.LogWarning("Ticker is null or empty in GetBuyingOptionAsync");
                throw new ArgumentNullException(nameof(ticker));
            }

            var stock = await _repository.GetByTickerAsync(ticker);

            if (stock == null)
            {
                _logger?.LogDebug("Stock not found for ticker {Ticker} in GetBuyingOptionAsync", ticker);
                return null;
            }

            return stock;
        }

        public async Task<BuyingOption?> GetBuyingOptionAsync(string ticker, decimal budget)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                _logger?.LogWarning("Ticker is null or empty in GetBuyingOptionAsync");
                throw new ArgumentNullException(nameof(ticker));
            }

            if (budget <= 0)
            {
                _logger?.LogWarning("Budget is less than or equal to zero in GetBuyingOptionAsync");
                throw new ArgumentOutOfRangeException(nameof(budget), "Budget must be greater than zero.");
            }

            var stock = await _repository.GetByTickerAsync(ticker);


            if (stock == null)
            {
                _logger?.LogDebug("Stock not found for ticker {Ticker} in GetBuyingOptionAsync", ticker);
                return null;
            }

            return new BuyingOption
            {
                Ticker = stock.Ticker,
                Shares = stock.Close > 0 ? (int)Math.Floor(budget / stock.Close) : 0,
                Budget = budget,
            };
        }
    }
}
