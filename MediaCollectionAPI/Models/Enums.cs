namespace MediaCollectionAPI.Models
{
    public enum MediaType
    {
        video_game,
        dvd,
        vhs,
        cd,
        book,
        manga
    }

    public enum MediaStatus
    {
        owned,
        wishlist,
        sold,
        loaned,
        lost
    }
}