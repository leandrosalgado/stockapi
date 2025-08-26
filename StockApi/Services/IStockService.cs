using StockApi.Models;

namespace StockApi.Services
{
    public interface IStockService
    {
        // TODO: Using IEnumerable to keep it simple but in a bigger app I would consider using IAsyncEnumerable or IQueryable
        // For streaming large datasets or for more complex querying
        // Also using async so if the code transitions to use a database or external service it will be easier to migrate
        Task<IEnumerable<string>> GetAllTickersAsync();
        Task<Stock?> GetStockByTickerAsync(string ticker);
        Task<BuyingOption?> GetBuyingOptionAsync(string ticker, decimal budget);
    }
}
