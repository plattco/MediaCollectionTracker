using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration; 
using System.Collections.Generic; 
public class IgdbGame
{
    public int id { get; set; }
    public string name { get; set; }
    public IgdbCover cover { get; set; }
    public long? first_release_date { get; set; }
    public List<IgdbPlatform> platforms { get; set; }
}

public class IgdbCover
{
    public string url { get; set; }
}

public class IgdbPlatform
{
    public string name { get; set; }
}

public class IgdbToken
{
    public string access_token { get; set; }
    public int expires_in { get; set; }
    public string token_type { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ExternalDataController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalDataController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Endpoint to search for games on IGDB
    [HttpGet("search/games")]
    public async Task<IActionResult> SearchGames([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term cannot be empty.");
        }

        var client = _httpClientFactory.CreateClient();

        // 1. Get an Access Token from Twitch/IGDB
        var clientId = _configuration["IGDB:ClientId"];
        var clientSecret = _configuration["IGDB:ClientSecret"];
        var tokenResponse = await client.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", null);
        
        if (!tokenResponse.IsSuccessStatusCode)
        {
            return StatusCode(500, "Failed to authenticate with IGDB.");
        }

        var token = await tokenResponse.Content.ReadFromJsonAsync<IgdbToken>();

        // 2. Use the Access Token to search for games
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Client-ID", clientId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

        var requestBody = $"search \"{term}\"; fields name, cover.url, first_release_date, platforms.name; limit 20;";
        var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");

        var searchResponse = await client.PostAsync("https://api.igdb.com/v4/games", content);

        if (!searchResponse.IsSuccessStatusCode)
        {
            return StatusCode(500, "Failed to search for games on IGDB.");
        }

        var games = await searchResponse.Content.ReadFromJsonAsync<List<IgdbGame>>();
        
        // Clean up the image URLs to get full-size images
        if (games != null)
        {
            foreach (var game in games)
            {
                if (game.cover != null && game.cover.url != null)
                {
                    game.cover.url = game.cover.url.Replace("t_thumb", "t_cover_big");
                }
            }
        }

        return Ok(games);
    }

    // Endpoint to get the resale value from eBay's sold listings
    [HttpGet("value")]
    public async Task<IActionResult> GetValue([FromQuery] string title, [FromQuery] string platform)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Title is required to find value.");
        }

        var client = _httpClientFactory.CreateClient();
        var appId = _configuration["Ebay:AppId"];
        
        // Construct a specific search query for better results
        var searchQuery = $"{title} {platform ?? ""}";

        // Call eBay's Finding API to get recently SOLD items
        var url = $"https://svcs.ebay.com/services/search/FindingService/v1?" +
                  $"OPERATION-NAME=findCompletedItems&" +
                  $"SERVICE-VERSION=1.0.0&" +
                  $"SECURITY-APPNAME={appId}&" +
                  $"RESPONSE-DATA-FORMAT=JSON&" +
                  $"REST-PAYLOAD&" +
                  $"keywords={Uri.EscapeDataString(searchQuery)}&" +
                  $"itemFilter(0).name=SoldItemsOnly&itemFilter(0).value=true&" +
                  $"sortOrder=EndTimeSoonest";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(500, "Failed to get data from eBay.");
        }

        using var jsonStream = await response.Content.ReadAsStreamAsync();
        var doc = JsonDocument.Parse(jsonStream);
        var searchResult = doc.RootElement.GetProperty("findCompletedItemsResponse")[0].GetProperty("searchResult")[0];

        if (searchResult.TryGetProperty("item", out var items))
        {
            var prices = new List<decimal>();
            foreach (var item in items.EnumerateArray())
            {
                if (item.TryGetProperty("sellingStatus", out var sellingStatus) &&
                    sellingStatus[0].TryGetProperty("currentPrice", out var currentPrice) &&
                    currentPrice[0].TryGetProperty("__value__", out var priceValue) &&
                    decimal.TryParse(priceValue.GetString(), out var price))
                {
                    prices.Add(price);
                }
            }

            if (prices.Any())
            {
                var averagePrice = prices.Average();
                return Ok(new { price = averagePrice });
            }
        }
        
        return NotFound("Could not determine a market value from recent sales.");
    }
}