using System;
using System.Collections.Generic;

namespace MediaCollectionAPI.Models.DTOs
{
    public class MediaItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? MediaType { get; set; }
        public string? Platform { get; set; }
        public string? Status { get; set; }
        public decimal? Rating { get; set; }
        public decimal? Price { get; set; }
        public DateTime? PriceLastUpdated { get; set; }
        public bool IsFavorite { get; set; } = false;
        public int Quantity { get; set; }
        public string? Barcode { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Publisher { get; set; }
        public string? Genre { get; set; }
        public string? Condition { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateMediaItemDto
    {
        public string Title { get; set; }
        public string MediaType { get; set; }
        public string? Platform { get; set; }
        public string? Status { get; set; } = "Owned";
        public decimal? Rating { get; set; }
        public decimal? Price { get; set; }
        public bool IsFavorite { get; set; } = false;
        public int Quantity { get; set; } = 1;
        public string? Barcode { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Publisher { get; set; }
        public string? Genre { get; set; }
        public string? Condition { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateMediaItemDto : CreateMediaItemDto
    {
        public Guid Id { get; set; }
    }

    public class UpdatePriceDto
    {
        public decimal Price { get; set; }
    }

    public class CollectionStatsDto
    {
        public int TotalItems { get; set; }
        public decimal? TotalValue { get; set; }
        public int? TotalFavorites { get; set; }
        public List<MediaTypeStatsDto> ByMediaType { get; set; }
    }

    public class MediaTypeStatsDto
    {
        public string? MediaType { get; set; }
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? AverageRating { get; set; }
        public int? FavoritesCount { get; set; }
    }
}
