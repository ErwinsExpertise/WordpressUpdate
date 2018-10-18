/* @(#) Worker.cs
 * @Author: Erwin Evans
 * @Purpose: Does shit
 * 
 * Version 1.0.0
 * 
 * (c) Erwin Evans 2018, All rights reserved.
 */

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WordpressUpdate
{
    class Worker
    {
        public static HtmlWeb wc = new HtmlWeb();

        public static string getVersion(string url)
        {
            var bldr = new StringBuilder();
            string[] target = File.ReadAllLines(@".\list.txt");
            foreach (string line in target)
            {
                var uri = new Uri(line);

                HtmlDocument doc = wc
                                    .Load(uri);

                string version = doc
                                   .DocumentNode
                                   .SelectNodes("//*/div/div/div/p[2]/text()")[0]
                                   .InnerText;

                string plugin = line
                                    .Replace("https://www.gplvault.com/product/", "");

                plugin = plugin
                              .Trim('/');

                printVersion(plugin + " - " + version);


                bldr
                    .Append(plugin + "-" +version + Environment.NewLine);

            }

            return bldr.ToString();
        }//end getVersion



        private static void printVersion(string version)
        {
            File.AppendAllText(@".\version.txt", version + Environment.NewLine);

        }//end printVersion

        public static async Task getUpdateAsync(string username, string password)
        {

            try {
                var baseAddress = new Uri("https://www.gplvault.com");
                var cookieContainer = new CookieContainer();
                var httpResponse = new HttpResponseMessage();
                var httpRequest = new HttpRequestMessage();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = false })
                using (var client = new HttpClient(handler, false) { BaseAddress = baseAddress })
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Referer", "https://www.gplvault.com/my-account/");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");


                    //Console.WriteLine("1: " + cookieContainer.GetCookieHeader(baseAddress));

                    var homePageResult = client.GetAsync("/");
                    homePageResult.Result.EnsureSuccessStatusCode();

                    var content = new FormUrlEncodedContent(new[]
                    {
                     new KeyValuePair<string, string>("username", username),
                     new KeyValuePair<string, string>("password", password),
                          });
                    //httpResponse = await client.PostAsync("/wp-admin", content);
                    var result = await client.PostAsync("/wp-login.php", content);
                    result.EnsureSuccessStatusCode();


                    Console.WriteLine("Current Cookie: " + cookieContainer.GetCookieHeader(baseAddress));

                    //var loginResult = client.PostAsync("/my-account/orders/", content).Result;
                    /////////////////////////////
                    cookieContainer.Add(new Cookie("_ga", "GA1.2.1355145636.1534278108") { Domain = baseAddress.Host });
                    cookieContainer.Add(new Cookie("_gid", "GA1.2.584278352.1534869071") { Domain = baseAddress.Host });
                    
                    string authCookie = 
                        username 
                        + "%" + "7C1535826289"
                        + "%" + "7CpQkjhsdflaSDFw4fsdgasGDsa45435egfFTert"
                        + "%" + "734DSe5tsGSdfg35hj34l5j3fsdfsdfsdr2324234dgsdfgsd";

                    cookieContainer.Add(new Cookie("wordpress_logged_in_13bvs887g65sd8796f57722345fe5", authCookie) { Domain = baseAddress.Host } );
                    
                    ////////////////////////////

                    string[] target = File.ReadAllLines(@".\list.txt");
                        var plug = new List<string>();
                        foreach (string line in target)
                        {
                            string plugin = line
                                                .Replace("https://www.gplvault.com/product/", "");

                            plugin = plugin
                                          .Trim('/');
                        plug.Add(plugin);
                        }




                    //Download Files
                    string[] download = getDownload().Split('/');

                    int i = 0;
                    foreach (string link in download)
                    {
                        if (i >= plug.Count)
                        {
                            break;
                        }
                            Console.WriteLine("Downloading: " + plug[i] + "...");
                            string path = @".\" + plug[i] + ".zip";
                            string dlink = "https://www.gplvault.com/download/" + link + "/" + plug[i] + ".zip";
                            var uri = new Uri(dlink);

                            httpResponse = await client.GetAsync(uri);

                            var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                            using (fs)
                            {
                                await httpResponse.Content.CopyToAsync(fs);
                            }
                            i++;
                       
                        
                    }


                }
            } catch(WebException ex)
                {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }


        }//end getUpdate

            public static string getDownload()
        {
            var bldr = new StringBuilder();
            string[] target = File.ReadAllLines(@".\list.txt");
            foreach (string line in target)
            {
                var uri = new Uri(line);
                HtmlDocument doc = wc.Load(uri);
                var aTags = doc.DocumentNode.SelectNodes("//*/div/div/div/p[7]/u");
                var hrefs = aTags.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList();
                string link = string.Join(Environment.NewLine, hrefs);
                link = link.Replace("https://www.gplvault.com/download/", "");
                bldr.Append(link);
            }

            return bldr.ToString();

        }//end getDownload





    }//end Worker
}
