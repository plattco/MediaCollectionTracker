namespace MediaCollectionAPI.Models
{
    public enum MediaType
    {
        VideoGame,
        DVD,
        VHS,
        CD,
        Book,
        Manga
    }

    public enum MediaStatus
    {
        Owned,
        Wishlist,
        Sold,
        Loaned,
        Lost
    }
}