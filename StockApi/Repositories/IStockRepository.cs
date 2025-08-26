using StockApi.Models;

namespace StockApi.Repositories
{
    public interface IStockRepository
    {
        // TODO: Using IEnumerable to keep it simple but in a bigger app I would consider using IAsyncEnumerable or IQueryable
        // For streaming large datasets or for more complex querying
        Task<IEnumerable<string>> GetAllAsync();
        Task<Stock?> GetByTickerAsync(string ticker);
    }
}
