using HtmlAgilityPack;
using UAMangaAPI.Models;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using UAMangaAPI.Data;

namespace UAMangaAPI.Services
{
    public class ParseService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IServiceScopeFactory _scopeFactory;

        public ParseService(IServiceProvider services, IServiceScopeFactory scopeFactory)
        {
            _services = services;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                GetAll();
                await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            }
        }

        public async Task GetAll()
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UAMangaAPIDbContext>();
                var dbValues = dbContext.Mangas;
                var parseRes = GetMalyopus().Concat(GetNashaIdea());
                foreach (var manga in parseRes)
                {
                    if (!dbValues.Select(x => x.Name).Contains(manga.Name))
                    {
                        dbValues.Add(manga);
                    }
                    else
                    {
                        var existing = dbValues.First(x => x.Name == manga.Name);
                        existing.Publisher = manga.Publisher;
                        existing.Price = manga.Price;
                        existing.CoverUrl = manga.CoverUrl;
                        existing.Link = manga.Link;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

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
                var manga = new Manga(name, coverUrl, price, "NashaIdea", link);

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
                    var manga = new Manga(name, coverUrl, price, "Malyopus", link);

                    // adding the object containing the scraped data to the list 
                    pageProducts.Add(manga);
                }
            }

            return pageProducts;
        }
    }
}
