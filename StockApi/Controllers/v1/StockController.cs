using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StockApi.Services;

namespace StockApi.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class StockController : ControllerBase
{
    private readonly ILogger<StockController> _logger;
    private readonly IStockService _stockService;
    public StockController(ILogger<StockController> logger, IStockService stockService)
    {
        _logger = logger;
        _stockService = stockService;
    }

    [HttpGet("tickers")]
    public async Task<IActionResult> GetAllTickers()
    {
        _logger.LogDebug("Getting all tickers.");
        var tickers = await _stockService.GetAllTickersAsync();
        _logger.LogDebug("Found {Count} tickers.", tickers?.Count() ?? 0);
        return Ok(tickers);
    }

    [HttpGet("{ticker}")]
    public async Task<IActionResult> GetTickerDetails(string ticker)
    {
        _logger.LogDebug("Getting details for ticker: {Ticker}", ticker);

        if (string.IsNullOrWhiteSpace(ticker))
        {
            _logger.LogWarning("Ticker parameter is null or empty.");
            return BadRequest("Ticker cannot be null or empty.");
        }

        var stock = await _stockService.GetStockByTickerAsync(ticker);
        if (stock == null)
        {
            _logger.LogWarning("Ticker not found: {Ticker}", ticker);
            return NotFound();
        }

        _logger.LogDebug("Returning details for ticker: {Ticker}", ticker);
        return Ok(stock);
    }


    // TODO: The task description says that the endpoint should have the possibility to "buy", if I understand correct 
    //       then this endpoint should be a post or a put. As the code I got is using [GET] so this endpoint is just calculating the
    //       Buying options and not mutating the list of stocks and their "available" shares as we don't have this information from the stock.json
    [HttpGet("{ticker}/buy")]
    public async Task<IActionResult> GetBuyingOption(string ticker, [FromQuery] decimal budget)
    {
        _logger.LogDebug("Getting buying option for ticker: {Ticker} with budget: {Budget}", ticker, budget);

        var buyingOption = await _stockService.GetBuyingOptionAsync(ticker, budget);

        if (buyingOption == null)
        {
            _logger.LogWarning("Buying option not found for ticker: {Ticker} with budget: {Budget}", ticker, budget);
            return NotFound();
        }

        _logger.LogDebug("Returning buying option for ticker: {Ticker} with budget: {Budget}", ticker, budget);
        return Ok(buyingOption);
    }
}