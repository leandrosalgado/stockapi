namespace StockApi.Models
{
    public class BuyingOption
    {
        public string Ticker { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public int Shares { get; set; }
    }
}
