namespace UAMangaAPI.Models
{
    public class Manga
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? CoverUrl { get; set; }
        public string? Price { get; set; }
        public string? Publisher { get; set; }
        public string? Link { get; set; }

        public Manga(string name, string coverUrl, string price, string publisher, string link)
        {
            Name = name;
            CoverUrl = coverUrl;
            Price = price;
            Publisher = publisher;
            Link = link;
            Id = Guid.NewGuid();
        }
    }
}
