using System;
using System.Data;
using System.Formats.Asn1;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Magazine.Models;

namespace SteamParser
{

    class SteamParser
    {
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            List<string> urls = new List<string>();
            List<Game> games = new List<Game>();
            urls = getUrls("https://store.steampowered.com/search/?os=win&hidef2p=1&filter=popularnew");
            
            foreach (string url in urls)
            { 
                games.Add(Parse(url));
            }
           
        }
        
        public static Game Parse(String url)
        {
            try
            {
               Game game = new Game();
                using (HttpClientHandler hdl = new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None })
                {
                    using (var client = new HttpClient(hdl))
                    {
                        using (HttpResponseMessage response = client.GetAsync(url).Result)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var html = response.Content.ReadAsStringAsync().Result;
                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);

                                    var img = doc.DocumentNode.SelectSingleNode(".//img[@class='game_header_image_full']").GetAttributeValue("src", "");
                                   
                                    
                                    game.imgSrc = img.ToString();
                                    game.name = doc.DocumentNode.SelectSingleNode(".//div[@class='apphub_AppName']").GetDirectInnerText();
                                    if( doc.DocumentNode.SelectSingleNode(".//div[@class='game_purchase_price price']").GetDirectInnerText() == null)
                                    {
                                        game.price = doc.DocumentNode.SelectSingleNode(".//div[@class='discount_original_price']").GetDirectInnerText().Trim();
                                    }
                                    else
                                    {
                                        game.price = doc.DocumentNode.SelectSingleNode(".//div[@class='game_purchase_price price']").GetDirectInnerText().Trim();
                                    }


                                    var allTags = doc.DocumentNode.SelectNodes(".//div[@class='glance_tags popular_tags']//a[@class='app_tag']");
                                    List<string> tags = new List<string>(3);
                                    foreach(var tag in allTags)
                                    {
                                        tags.Add(tag.InnerText.Trim());
                                    }
                                    game.describe = doc.DocumentNode.SelectSingleNode(".//div[@class='game_description_snippet']").GetDirectInnerText().Trim();
                                    game.tags = tags;
                                    game.Print();
                                    return game;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { }
            return null;
        }

        public static List<string> getUrls(string url)
        {
            try
            {
                List<string> result = new List<string>();
                using (HttpClientHandler hdl = new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None })
                {
                    using (var client = new HttpClient(hdl))
                    {
                        using (HttpResponseMessage response = client.GetAsync(url).Result)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var html = response.Content.ReadAsStringAsync().Result;
                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                    var urls = doc.DocumentNode.SelectNodes(".//div[@id='search_resultsRows']//a");

                                        foreach (var bebra in urls)
                                        {
                                            result.Add(bebra.GetAttributeValue("href", ""));
                                           
                                        }
                                 
                                }

                            }
                        }


                    }

                }
                return result;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;


        }
    }
}