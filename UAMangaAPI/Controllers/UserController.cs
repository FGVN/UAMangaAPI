using Microsoft.AspNetCore.Mvc;
using System.Runtime.ExceptionServices;
using UAMangaAPI.Data;
using UAMangaAPI.Models;

namespace UAMangaAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public ILogger<MangaController> _logger { get; }
        public UAMangaAPIDbContext dbContext { get; }
        public UserController(ILogger<MangaController> logger, UAMangaAPIDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet("GetInfo/{userId}")]
        public IActionResult Get(string userId)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest("User with such id does not exist");
        }



        [HttpPost("/Register/{username}/{password}")]
        public IActionResult RegisterUser(string username, string password)
        {
            if (dbContext.Users.Select(x => x.UserName).Contains(username))
                return BadRequest("Username already taken");

            User toAdd = new User(username, Services.SecurityService.HashPassword(password));

            dbContext.Add(toAdd);
            dbContext.SaveChanges();
            return Ok(toAdd.Id);
        }

        [HttpPost("/Login/{username}/{password}")]
        public IActionResult LoginUser(string username, string password)
        {
            if (!dbContext.Users.Select(x => x.UserName).Contains(username))
                return BadRequest("User does not exist");

            User found = dbContext.Users.First(x => x.UserName == username);

            if (!Services.SecurityService.VerifyPassword(password, found.Password))
                return BadRequest("Wrong password");
            return Ok(found.Id);
        }
    }
}
