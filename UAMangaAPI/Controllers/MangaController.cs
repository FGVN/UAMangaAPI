using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Mvc;
using UAMangaAPI.Models;

namespace UAMangaAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MangaController : ControllerBase
    {
        private readonly ILogger<MangaController> _logger;

        public MangaController(ILogger<MangaController> logger)
        {
            _logger = logger;
        }


        [HttpGet("All")]
        public IEnumerable<Manga> GetAll() => GetMalyopus().ToList().Concat(GetNashaIdea().ToList());

        [HttpGet("Malyopus")]
        public IEnumerable<Manga> GetMalyopus()
        {
            List<Manga> resultMangas = new List<Manga>();
            int i = 1;
            List<Manga> pageContent = ParseMalyopusPage($"https://malopus.com.ua/manga?=page{1}").ToList();
            resultMangas.AddRange(pageContent);
            while (pageContent[0].Name != resultMangas[0].Name)
            {
                i++;
                pageContent = ParseMalyopusPage($"https://malopus.com.ua/manga?=page{i}").ToList();
                resultMangas.AddRange(pageContent);
            }
            return resultMangas;
        }

        [HttpGet("NashaIdea")]
        public IEnumerable<Manga> GetNashaIdea()
        {
            List<Manga> resultMangas = new List<Manga>();
            int i = 1;
            List<Manga> pageContent = ParseIdeaPage($"https://nashaidea.com/page/{1}").ToList();
            while (pageContent.Count > 0)
            {
                resultMangas.AddRange(pageContent);
                i++;
                pageContent = ParseIdeaPage($"https://nashaidea.com/page/{i}").ToList();
            }
            return resultMangas;
        }

        public static IEnumerable<Manga> ParseIdeaPage(string pageUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(pageUrl);

            List<Manga> pageProducts = new List<Manga>();


            var productHTMLElements = document.DocumentNode.QuerySelectorAll("li.product");

            foreach (var productHTMLElement in productHTMLElements)
            {
                // scraping the interesting data from the current HTML element 
                var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h3 a").InnerText);
                var coverUrl = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);
                var link = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a.woocommerce-LoopProduct-link").Attributes["href"].Value);

                // instancing a new Manga object 
                var manga = new Manga() { Name = name, CoverUrl = coverUrl, Link = link, Price = price, Publisher = "NashaIdea" };

                // adding the object containing the scraped data to the list 
                pageProducts.Add(manga);
            }

            return pageProducts;
        }
        public static IEnumerable<Manga> ParseMalyopusPage(string pageUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(pageUrl);

            List<Manga> pageProducts = new List<Manga>();

            var productHTMLElements = document.DocumentNode.SelectNodes("//div[@class='product-layout product-grid rm-module-col col-12 col-md-4 col-lg-4']");

            if (productHTMLElements != null)
            {
                foreach (var productHTMLElement in productHTMLElements)
                {
                    // scraping the interesting data from the current HTML element 
                    var name = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//div[@class='rm-module-title']/a").InnerText);
                    var coverUrl = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//a[@class='order-0']/img").Attributes["src"].Value);
                    var price = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//span[@class='rm-module-price']").InnerText);
                    var link = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//div[@class='rm-module-title']/a").Attributes["href"].Value);

                    // instancing a new Manga object 
                    var manga = new Manga() { Name = name, CoverUrl = coverUrl, Link = link, Price = price, Publisher = "Malyopus" };

                    // adding the object containing the scraped data to the list 
                    pageProducts.Add(manga);
                }
            }

            return pageProducts;
        }


    }
}
