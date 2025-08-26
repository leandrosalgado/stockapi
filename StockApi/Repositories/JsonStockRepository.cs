using StockApi.Converters;
using StockApi.Models;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace StockApi.Repositories
{
    /// <summary>   
    /// A repository that loads stock data from a JSON file and provides methods to access the data.
    /// </summary>
    public class JsonStockRepository(ILogger<JsonStockRepository> logger) : IStockRepository
    {
        private readonly ILogger<JsonStockRepository> _logger = logger;
        // Initialize an empty list of stocks to ensure the repo works fine even if the LoadFromJsonPath was not called
        private ConcurrentDictionary<string, Stock> _stocks = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Loads stock data from a JSON file at the specified path.
        /// </summary>
        /// <param name="jsonPath">Valid JSON path</param>
        /// <param name="ignoreInvalid">If this parameter is false any invalid entry will trigger a validation exception and 
        /// the process will be stopped and the list of stocks emptied</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="JsonException"></exception>
        public void LoadFromJonPath(string jsonPath, bool ignoreInvalid = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonPath))
                {
                    throw new ArgumentNullException(nameof(jsonPath));
                }

                var pathInvalidChars = Path.GetInvalidPathChars();

                if (jsonPath.Any(c => pathInvalidChars.Contains(c)))
                {
                    var message = $"JSON path contains invalid characters: {jsonPath}";
                    _logger?.LogWarning(message);
                    throw new ArgumentException(message, nameof(jsonPath));
                }

                if (!File.Exists(jsonPath))
                {
                    var message = $"JSON file not found at path: {jsonPath}";
                    var fileName = Path.GetFileName(jsonPath);
                    _logger?.LogWarning(message);
                    throw new FileNotFoundException(message, fileName);
                }

                using var jsonStream = File.OpenRead(jsonPath);

                // Although we have the converter registered globally using DI
                // to keep this class self-contained and decouple for tests we register it here as well
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                options.Converters.Add(new DateOnlyJsonConverter());

                var stocks = JsonSerializer.Deserialize<List<Stock>>(jsonStream, options) ?? new List<Stock>();

                var validStocks = new List<Stock>();
                // Initialize the dict so we have a clean state on each load
                _stocks = new ConcurrentDictionary<string, Stock>(StringComparer.OrdinalIgnoreCase);
                foreach (var stock in stocks)
                {
                    var context = new ValidationContext(stock);
                    var results = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(stock, context, results, true);
                    if (isValid && !_stocks.ContainsKey(stock.Ticker))
                    {
                        _stocks.TryAdd(stock.Ticker, stock);
                    }
                    else if (!ignoreInvalid)
                    {
                        var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                        // In case of invalid data we clear the dict to avoid partial loads
                        _stocks.Clear();
                        throw new ValidationException($"Invalid stock entry: {errorMessages}");
                    }
                    else
                    {
                        _logger?.LogWarning("Invalid stock entry ignored: {Errors}", string.Join("; ", results.Select(r => r.ErrorMessage)));
                    }
                }
            }
            catch (JsonException ex)
            {
                // Log different message for deserialization issues to help troubleshooting
                _logger?.LogError(ex, $"JSON deserialization failed for the file {jsonPath}");
                throw; // rethrow so the caller can handle it
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to load {jsonPath}: {ex?.Message}");
                throw;
            }
        }

        public Task<IEnumerable<string>> GetAllAsync()
        {
            try
            {
                _logger?.LogDebug("Retrieving all stock tickers.");
                return Task.FromResult(_stocks.Keys.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error Getting all tickers");

                throw;
            }

        }

        public Task<Stock?> GetByTickerAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            if (_stocks.TryGetValue(ticker, out var stock))
            {
                return Task.FromResult<Stock?>(stock);
            }
            return Task.FromResult<Stock?>(null);
        }
    }
}
