using HtmlAgilityPack;
using UAMangaAPI.Models;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using UAMangaAPI.Data;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;

namespace UAMangaAPI.Services
{
    public class ParseService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IServiceScopeFactory _scopeFactory;

        private static HtmlWeb web = new HtmlWeb();

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
                var parseRes = GetMalyopus().
                        Concat(GetNashaIdea()).
                        Concat(GetMolfar()).
                        Concat(GetSafran()).
                        Concat(GetLantsuta()).
                    OrderBy(manga => manga.Name);
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
            List<Manga> pageContent = ParseIdeaPage($"https://nashaidea.com/product-category/manga/page/{1}").ToList();
            while (pageContent.Count > 0)
            {
                resultMangas.AddRange(pageContent);
                i++;
                pageContent = ParseIdeaPage($"https://nashaidea.com/product-category/manga/page/{i}").ToList();
            }
            return resultMangas;
        }

        public IEnumerable<Manga> GetMolfar()
        {

            List<Manga> resultMangas = new List<Manga>();
            int i = 1;
            List<Manga> pageContent = ParseMolfarPage($"https://molfar-comics.com/product-category/manga/page/{i}").ToList();
            while (pageContent.Count > 0)
            {
                resultMangas.AddRange(pageContent);
                i++;
                pageContent = ParseMolfarPage($"https://molfar-comics.com/product-category/manga/page/{i}").ToList();
            }
            return resultMangas;
        }

        public IEnumerable<Manga> GetSafran() => ParseSafranPage($"https://safranbook.com/catalog/genre/comics/").ToList();

        public IEnumerable<Manga> GetLantsuta()
        {

            List<Manga> resultMangas = new List<Manga>();
            int i = 1;
            List<Manga> pageContent = ParseLantsutaPage($"https://lantsuta-publishing.com/manga?page={i}").ToList();
            while (pageContent.Count > 0)
            {
                resultMangas.AddRange(pageContent);
                i++;
                pageContent = ParseLantsutaPage($"https://lantsuta-publishing.com/manga?page={i}").ToList();
            }
            return resultMangas;
        }
        

        public static IEnumerable<Manga> ParseIdeaPage(string pageUrl)
        {
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
        public IEnumerable<Manga> ParseMolfarPage(string pageUrl)
        {
            HtmlDocument document = web.Load(pageUrl);

            List<Manga> pageProducts = new List<Manga>();

            var productHTMLElements = document.DocumentNode.SelectNodes("//div[contains(@class, 'style-grid') and contains(@class, 'product')]");
            if(productHTMLElements == null)
                return pageProducts;


            foreach (var productHTMLElement in productHTMLElements)
            {
                var titleNode = productHTMLElement.SelectSingleNode(".//a[contains(@class, 'wrapTitle')]/text()");

                if (titleNode != null)
                {
                    var name = HtmlEntity.DeEntitize(titleNode.InnerText.Trim());
                    var coverUrl = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//a[@class='wrapImage']/img").Attributes["data-lazy-src"].Value);
                    var price = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//span[@class='priceBtn']").InnerText.Trim());
                    var link = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//a[contains(@class, 'wrapTitle')]").Attributes["href"].Value);

                    var manga = new Manga(name, coverUrl, price, "Molfar", link);
                    pageProducts.Add(manga);
                }
            }


            return pageProducts;
        }
        public IEnumerable<Manga> ParseSafranPage(string pageUrl)
        {
            HtmlDocument document = web.Load(pageUrl);

            List<Manga> pageProducts = new List<Manga>();

            var productHTMLElements = document.DocumentNode.SelectNodes("//div[contains(@class, 'products-item')]");
            if (productHTMLElements == null)
                return pageProducts;

            foreach (var productHTMLElement in productHTMLElements)
            {
                var titleNode = productHTMLElement.SelectSingleNode(".//div[@class='products-item-name']/a");
                var priceNode = productHTMLElement.SelectSingleNode(".//div[@class='products-item-bottom']//div[@class='products-item-price']"); 
                var imgNode = productHTMLElement.SelectSingleNode(".//img[@class='products-item-img']");



                var name = HtmlEntity.DeEntitize(titleNode?.InnerText.Trim() ?? "");
                var coverUrl = "https://safranbook.com" + HtmlEntity.DeEntitize(imgNode?.GetAttributeValue("src", "") ?? "");
                var price = HtmlEntity.DeEntitize(priceNode?.InnerText ?? "");
                var link = "https://safranbook.com" + HtmlEntity.DeEntitize(titleNode?.GetAttributeValue("href", "") ?? "");

                var manga = new Manga(name, coverUrl, price, "Safran", link);

                if (manga.Name != "" && Regex.IsMatch(manga.Name, @"Том\d+"))
                    pageProducts.Add(manga);
            }

            return pageProducts;
        }
        public IEnumerable<Manga> ParseLantsutaPage(string pageUrl)
        {
            HtmlDocument document = web.Load(pageUrl);

            List<Manga> pageProducts = new List<Manga>();

            var productHTMLElements = document.DocumentNode.SelectNodes("//div[@class='product-layout product-grid col-xs-12 col-md-4']");

            if(productHTMLElements == null)
                return pageProducts;

            foreach (var productHTMLElement in productHTMLElements)
            {
                var name = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//h3[@class='name-product']/a").InnerText);
                var coverUrl = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//div[@class='image']/a/img").Attributes["src"].Value);
                var price = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//p[@class='price']").InnerText);
                var link = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//div[@class='button-box']/a[@class='btn-more']").Attributes["href"].Value);

                var manga = new Manga(name, coverUrl, price, "Lantsuta", link);

                if(!manga.Name.Contains("Комплект"))
                    pageProducts.Add(manga);
            }

            return pageProducts;
        }



    }
}
