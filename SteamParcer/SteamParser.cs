using System;
using System.Data;
using System.Formats.Asn1;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace SteamParser
{

    class SteamParser
    {

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            List<string> urls = new List<string>();
            urls = getUrls("https://store.steampowered.com/search/?os=win&hidef2p=1&filter=popularnew");
            //dsf
            //foreach (string url in urls)
            //{
            //    Parse(url);
            //}
        }
        
        public static object Parse(String url)
        {
            try
            {
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
                                   
                                    
                                    var imgUrl = img.ToString();
                                    var title = doc.DocumentNode.SelectSingleNode(".//div[@class='apphub_AppName']").GetDirectInnerText();
                                    var price = doc.DocumentNode.SelectSingleNode(".//div[@class='discount_original_price']").GetDirectInnerText();
                                    
                                    var allTags = doc.DocumentNode.SelectNodes(".//div[@class='glance_tags popular_tags']//a[@class='app_tag']");
                                    List<string> tags = new List<string>();
                                    foreach(var tag in allTags)
                                    {
                                        tags.Add(tag.InnerText.Trim());
                                        
                                    }
                                    var describe = doc.DocumentNode.SelectSingleNode(".//div[@class='game_description_snippet']").GetDirectInnerText().Trim();
                                    Console.WriteLine(describe);
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
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
                                    int i = 0;
                                    foreach(var url1 in result)
                                    {
                                        Console.WriteLine(url1);
                                        i++;
                                    }
                                    Console.WriteLine(i);

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