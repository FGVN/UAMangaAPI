using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UAMangaAPI.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> Wishlist { get; set; } = new List<string>();
        public List<string> Own { get; set; } = new List<string>();

        public User()
        {
            // Parameterless constructor
            Id = Guid.NewGuid().ToString();
        }

        public User(string username, string password) : this()
        {
            // Constructor with parameters
            UserName = username;
            Password = password;
        }

        //// Nav props
        //public ICollection<UserManga> WishlistMangas { get; set; } = new List<UserManga>();
        //public ICollection<UserManga> OwnMangas { get; set; } = new List<UserManga>();
    }

}
