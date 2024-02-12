namespace UAMangaAPI.Models
{
    public class UserManga
    {
        public string UserId { get; set; }  // Change the type to string
        public string MangaId { get; set; }

        public User User { get; set; }
        public Manga Manga { get; set; }
    }
}
