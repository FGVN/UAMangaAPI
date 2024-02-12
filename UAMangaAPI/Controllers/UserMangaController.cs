using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UAMangaAPI.Data;
using UAMangaAPI.Models;

namespace UAMangaAPI.Controllers
{
    public class UserMangaController : ControllerBase
    {
        public ILogger<MangaController> _logger { get; }
        public UAMangaAPIDbContext dbContext { get; }
        public UserMangaController(ILogger<MangaController> logger, UAMangaAPIDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        [HttpPost("/Wishlist/Add/{userId}/{mangaId}")]
        public async Task<IActionResult> WishlistAddAsync(string userId, string mangaId)
        {
            if (!dbContext.Users.Select(x => x.Id).Contains(userId))
                return BadRequest("User does not exist");
            if (!dbContext.Mangas.Select(x => x.Id).Contains(mangaId))
                return BadRequest("Manga does not exist");

            var user = dbContext.Users.Find(userId);
            if(user.Wishlist.Contains(mangaId))
                return BadRequest("Manga is already present");

            user.Wishlist.Add(mangaId);

                // Mark user as modified to ensure changes are tracked
            dbContext.Entry(user).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("/Wishlist/Remove/{userId}/{mangaId}")]
        public async Task<IActionResult> WishlistRemoveAsync(string userId, string mangaId)
        {
            if (!dbContext.Users.Any(x => x.Id == userId && x.Wishlist.Contains(mangaId)))
                return BadRequest("There is no such a wishlist item");

            var user = dbContext.Users.Find(userId);
            user.Wishlist.Remove(mangaId);
            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("/Own/Add/{userId}/{mangaId}")]
        public async Task<IActionResult> OwnAddAsync(string userId, string mangaId)
        {
            if (!dbContext.Users.Select(x => x.Id).Contains(userId))
                return BadRequest("User does not exist");
            if (!dbContext.Mangas.Select(x => x.Id).Contains(mangaId))
                return BadRequest("Manga does not exist");

            var user = dbContext.Users.Find(userId);
            if (user.Own.Contains(mangaId))
                return BadRequest("Manga is already present");
            user.Own.Add(mangaId);

            // Mark user as modified to ensure changes are tracked
            dbContext.Entry(user).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("/Own/Remove/{userId}/{mangaId}")]
        public async Task<IActionResult> OwnRemoveAsync(string userId, string mangaId)
        {
            if (!dbContext.Users.Any(x => x.Id == userId && x.Own.Contains(mangaId)))
                return BadRequest("There is no such a wishlist item");

            var user = dbContext.Users.Find(userId);
            user.Own.Remove(mangaId);
            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
