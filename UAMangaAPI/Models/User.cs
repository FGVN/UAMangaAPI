using System.ComponentModel.DataAnnotations.Schema;

namespace UAMangaAPI.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public List<Guid> Wishlist { get; set; }
        [NotMapped]
        public List<Guid> Own { get; set; }
    }
}
