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


        [HttpGet("All")]
        public IEnumerable<Manga> GetAll() => dbContext.Mangas;

        [HttpGet("Malyopus")]
        public IEnumerable<Manga> GetMalyopus() => dbContext.Mangas.Where(x => x.Publisher == "Malyopus");

        [HttpGet("NashaIdea")]
        public IEnumerable<Manga> GetNashaIdea() => dbContext.Mangas.Where(x => x.Publisher == "NashaIdea");

        [HttpGet("Molfar")]
        public IEnumerable<Manga> GetMolfar() => dbContext.Mangas.Where(x => x.Publisher == "Molfar");

        [HttpGet("Safran")]
        public IEnumerable<Manga> GetSafran() => dbContext.Mangas.Where(x => x.Publisher == "Safran");

    }
}
