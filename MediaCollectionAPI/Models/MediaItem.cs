using System;
using System.ComponentModel.DataAnnotations;

namespace MediaCollectionAPI.Models
{
    public class MediaItem
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        
        [Required]
        public MediaType MediaType { get; set; }
        
        [MaxLength(100)]
        public string Platform { get; set; }
        
        public MediaStatus Status { get; set; } = MediaStatus.owned;
        
        [Range(0, 5)]
        public decimal? Rating { get; set; }
        
        [Range(0, 999999.99)]
        public decimal? Price { get; set; }
        
        public DateTime? PriceLastUpdated { get; set; }
        
        public bool IsFavorite { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 1;
        
        [MaxLength(50)]
        public string Barcode { get; set; }
        
        public int? ReleaseYear { get; set; }
        
        [MaxLength(200)]
        public string Publisher { get; set; }
        
        [MaxLength(100)]
        public string Genre { get; set; }
        
        [MaxLength(50)]
        public string Condition { get; set; }
        
        public string Notes { get; set; }
        
        [MaxLength(500)]
        public string ImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}