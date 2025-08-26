using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using StockApi.Converters;
using StockApi.Repositories;
using StockApi.Services;


// TODO: Remove server headers such as Kestrel and etc ( can be handled in a middleware in the API or in the reverse proxy)
// TODO: Compression and things like that also could be configured in the API or handled by a proxy
// TODO: Authentication etc
// TODO: Middleware with error handling customization
// TODO: Cloud Logging configuration, currently the solution uses ILogger interface but the logging goes nowhere.
// TODO: Add Forward headers (UseForwardedHeaders) and base path (UsePathBase) in case the API runs behind a proxy
// TODO: Other security middlewares such as UseHsts and UseCors or leave these things to the proxy configuration
// TODO: Comment more the classes and methods

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

// Add logging
services.AddLogging();

services.AddScoped<IStockService, StockService>();
services.AddSingleton<IStockRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<JsonStockRepository>>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var jsonPath = Path.Combine(env.ContentRootPath, "stocks.json");
    var jsRepo = new JsonStockRepository(logger);

    jsRepo.LoadFromJonPath(jsonPath);

    return jsRepo;
});


services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        });
;

// As it's a new API I always add versioning to ensure if an API redesign is needed we can plan a migration and sunset endpoints as needed
services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = false;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.ConfigureOptions<ConfigureSwaggerOptions>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
