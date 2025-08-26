using System.ComponentModel.DataAnnotations;

namespace StockApi.Models
{
    /// <summary>
    /// Represents stock market data for a specific ticker symbol.
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Stock ticker symbol (e.g., AAPL for Apple Inc.)
        /// </summary>
        [Required]
        [Key]
        public required string Ticker { get; set; }
        public DateOnly Date { get; set; }
        /// <summary>
        /// Price at market open
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(Open)} price must be non-negative.")]
        public decimal Open { get; set; }
        /// <summary>
        /// Price at market close
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(Close)} price must be non-negative.")]
        public decimal Close { get; set; }
        /// <summary>
        /// Highest price during the trading day
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(High)} price must be non-negative.")]
        public decimal High { get; set; }
        /// <summary>
        /// Lowest price during the trading day
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(Low)} price must be non-negative.")]
        public decimal Low { get; set; }
        /// <summary>
        /// Total number of shares traded during the day
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(Volume)} price must be non-negative.")]
        public long Volume { get; set; }
    }
}
