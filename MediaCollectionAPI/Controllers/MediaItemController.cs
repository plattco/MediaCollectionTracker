using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaCollectionAPI.Data;
using MediaCollectionAPI.Models;
using MediaCollectionAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaItemsController : ControllerBase
    {
        private readonly MediaCollectionContext _context;

        public MediaItemsController(MediaCollectionContext context)
        {
            _context = context;
        }

        // GET: api/MediaItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaItemDto>>> GetMediaItems(
            [FromQuery] string mediaType = null,
            [FromQuery] string platform = null,
            [FromQuery] string status = null,
            [FromQuery] bool? isFavorite = null,
            [FromQuery] string searchTerm = null,
            [FromQuery] string sortBy = "title",
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.MediaItems.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(mediaType))
                query = query.Where(m => m.MediaType.ToString() == mediaType);

            if (!string.IsNullOrEmpty(platform))
                query = query.Where(m => m.Platform == platform);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status.ToString() == status);

            if (isFavorite.HasValue)
                query = query.Where(m => m.IsFavorite == isFavorite.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(m => 
                    m.Title.ToLower().Contains(searchLower) || 
                    (m.Publisher != null && m.Publisher.ToLower().Contains(searchLower)) ||
                    (m.Genre != null && m.Genre.ToLower().Contains(searchLower)) ||
                    (m.Notes != null && m.Notes.ToLower().Contains(searchLower)));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "title" => descending ? query.OrderByDescending(m => m.Title) : query.OrderBy(m => m.Title),
                "rating" => descending ? query.OrderByDescending(m => m.Rating) : query.OrderBy(m => m.Rating),
                "price" => descending ? query.OrderByDescending(m => m.Price) : query.OrderBy(m => m.Price),
                "createdat" => descending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt),
                "updatedat" => descending ? query.OrderByDescending(m => m.UpdatedAt) : query.OrderBy(m => m.UpdatedAt),
                _ => query.OrderBy(m => m.Title)
            };

            // Apply pagination
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MediaItemDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    MediaType = m.MediaType.ToString(),
                    Platform = m.Platform,
                    Status = m.Status.ToString(),
                    Rating = m.Rating,
                    Price = m.Price,
                    PriceLastUpdated = m.PriceLastUpdated,
                    IsFavorite = m.IsFavorite,
                    Quantity = m.Quantity,
                    Barcode = m.Barcode,
                    ReleaseYear = m.ReleaseYear,
                    Publisher = m.Publisher,
                    Genre = m.Genre,
                    Condition = m.Condition,
                    Notes = m.Notes,
                    ImageUrl = m.ImageUrl,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToListAsync();

            // Add pagination info to response headers
            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return Ok(items);
        }

        // GET: api/MediaItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MediaItemDto>> GetMediaItem(Guid id)
        {
            var item = await _context.MediaItems.FindAsync(id);

            if (item == null)
                return NotFound();

            return Ok(new MediaItemDto
            {
                Id = item.Id,
                Title = item.Title,
                MediaType = item.MediaType.ToString(),
                Platform = item.Platform,
                Status = item.Status.ToString(),
                Rating = item.Rating,
                Price = item.Price,
                PriceLastUpdated = item.PriceLastUpdated,
                IsFavorite = item.IsFavorite,
                Quantity = item.Quantity,
                Barcode = item.Barcode,
                ReleaseYear = item.ReleaseYear,
                Publisher = item.Publisher,
                Genre = item.Genre,
                Condition = item.Condition,
                Notes = item.Notes,
                ImageUrl = item.ImageUrl,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            });
        }

        // POST: api/MediaItems
        [HttpPost]
        public async Task<ActionResult<MediaItemDto>> CreateMediaItem(CreateMediaItemDto dto)
        {
            if (!Enum.TryParse<MediaType>(dto.MediaType, out var mediaType))
                return BadRequest($"Invalid media type: {dto.MediaType}");

            if (!Enum.TryParse<MediaStatus>(dto.Status, out var status))
                return BadRequest($"Invalid status: {dto.Status}");

            var item = new MediaItem
            {
                Title = dto.Title,
                MediaType = mediaType,
                Platform = dto.Platform,
                Status = status,
                Rating = dto.Rating,
                Price = dto.Price,
                PriceLastUpdated = dto.Price.HasValue ? DateTime.UtcNow : null,
                IsFavorite = dto.IsFavorite,
                Quantity = dto.Quantity,
                Barcode = dto.Barcode,
                ReleaseYear = dto.ReleaseYear,
                Publisher = dto.Publisher,
                Genre = dto.Genre,
                Condition = dto.Condition,
                Notes = dto.Notes,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MediaItems.Add(item);
            await _context.SaveChangesAsync();

            var resultDto = new MediaItemDto
            {
                Id = item.Id,
                Title = item.Title,
                MediaType = item.MediaType.ToString(),
                Platform = item.Platform,
                Status = item.Status.ToString(),
                Rating = item.Rating,
                Price = item.Price,
                PriceLastUpdated = item.PriceLastUpdated,
                IsFavorite = item.IsFavorite,
                Quantity = item.Quantity,
                Barcode = item.Barcode,
                ReleaseYear = item.ReleaseYear,
                Publisher = item.Publisher,
                Genre = item.Genre,
                Condition = item.Condition,
                Notes = item.Notes,
                ImageUrl = item.ImageUrl,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return CreatedAtAction(nameof(GetMediaItem), new { id = item.Id }, resultDto);
        }

        // PUT: api/MediaItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMediaItem(Guid id, UpdateMediaItemDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var item = await _context.MediaItems.FindAsync(id);
            if (item == null)
                return NotFound();

            if (!Enum.TryParse<MediaType>(dto.MediaType, out var mediaType))
                return BadRequest($"Invalid media type: {dto.MediaType}");

            if (!Enum.TryParse<MediaStatus>(dto.Status, out var status))
                return BadRequest($"Invalid status: {dto.Status}");

            // Track if price changed
            bool priceChanged = item.Price != dto.Price;

            item.Title = dto.Title;
            item.MediaType = mediaType;
            item.Platform = dto.Platform;
            item.Status = status;
            item.Rating = dto.Rating;
            item.Price = dto.Price;
            
            // Update price timestamp if price changed
            if (priceChanged && dto.Price.HasValue)
            {
                item.PriceLastUpdated = DateTime.UtcNow;
            }
            
            item.IsFavorite = dto.IsFavorite;
            item.Quantity = dto.Quantity;
            item.Barcode = dto.Barcode;
            item.ReleaseYear = dto.ReleaseYear;
            item.Publisher = dto.Publisher;
            item.Genre = dto.Genre;
            item.Condition = dto.Condition;
            item.Notes = dto.Notes;
            item.ImageUrl = dto.ImageUrl;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MediaItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaItem(Guid id)
        {
            var item = await _context.MediaItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.MediaItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/MediaItems/5/price
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceDto dto)
        {
            var item = await _context.MediaItems.FindAsync(id);
            if (item == null)
                return NotFound();

            if (dto.Price < 0)
                return BadRequest("Price cannot be negative");

            item.Price = dto.Price;
            item.PriceLastUpdated = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { 
                price = item.Price, 
                priceLastUpdated = item.PriceLastUpdated 
            });
        }

        // GET: api/MediaItems/stats
        [HttpGet("stats")]
        public async Task<ActionResult<CollectionStatsDto>> GetCollectionStats()
        {
            var stats = await _context.MediaItems
                .Where(m => m.Status == MediaStatus.owned)
                .GroupBy(m => m.MediaType)
                .Select(g => new MediaTypeStatsDto
                {
                    MediaType = g.Key.ToString(),
                    TotalItems = g.Count(),
                    TotalQuantity = g.Sum(m => m.Quantity),
                    TotalValue = g.Sum(m => (m.Price ?? 0) * m.Quantity),
                    AverageRating = g.Average(m => m.Rating),
                    FavoritesCount = g.Count(m => m.IsFavorite)
                })
                .ToListAsync();

            var overallStats = new CollectionStatsDto
            {
                TotalItems = await _context.MediaItems.CountAsync(m => m.Status == MediaStatus.owned),
                TotalValue = await _context.MediaItems
                    .Where(m => m.Status == MediaStatus.owned)
                    .SumAsync(m => (m.Price ?? 0) * m.Quantity),
                TotalFavorites = await _context.MediaItems
                    .CountAsync(m => m.IsFavorite && m.Status == MediaStatus.owned),
                ByMediaType = stats
            };

            return Ok(overallStats);
        }

        // GET: api/MediaItems/platforms
        [HttpGet("platforms")]
        public async Task<ActionResult<IEnumerable<string>>> GetPlatforms()
        {
            var platforms = await _context.MediaItems
                .Where(m => !string.IsNullOrEmpty(m.Platform))
                .Select(m => m.Platform)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            return Ok(platforms);
        }

        // GET: api/MediaItems/genres
        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<string>>> GetGenres()
        {
            var genres = await _context.MediaItems
                .Where(m => !string.IsNullOrEmpty(m.Genre))
                .Select(m => m.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            return Ok(genres);
        }
    }
}