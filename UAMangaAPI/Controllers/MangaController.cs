using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using UAMangaAPI.Data;
using UAMangaAPI.Models;

namespace UAMangaAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MangaController : ControllerBase
    {
        private readonly ILogger<MangaController> _logger;
        private readonly UAMangaAPIDbContext dbContext;

        public MangaController(ILogger<MangaController> logger, UAMangaAPIDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IEnumerable<Manga> GetAll() => dbContext.Mangas;

        [HttpGet("{publisher}")]
        public IEnumerable<Manga> GetByPublisher(string publisher) => dbContext.Mangas.Where(x => x.Publisher == publisher);

        [HttpGet("Search/{name}")]
        public IEnumerable<Manga> GetByName(string name, string? publisher = null)
        {
            IQueryable<Manga> query = dbContext.Mangas.Where(x => x.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(publisher))
            {
                query = query.Where(x => x.Publisher == publisher);
            }

            return query.ToList();
        }

    }
}
