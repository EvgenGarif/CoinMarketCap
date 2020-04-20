using CoinMarketCap.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace CoinMarketCap.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult MarketCapInfo()
        {
            string data = "";
            List<MarketCap> MarketCapList = new List<MarketCap>();
            try
            {
                data = makeAPICall();
                JObject obj = JObject.Parse(data);

                foreach (var val in obj.SelectToken("data"))
                {
                    MarketCap marketCap = new MarketCap();

                    marketCap.Name = val.SelectToken("name").ToString();
                   
                    marketCap.Symbol = val.SelectToken("symbol").ToString();
                    
                
                    decimal price = Convert.ToDecimal(val.SelectToken("quote.USD.price"));
                    marketCap.Price = "$" + Math.Round(price, 2);

                    decimal changeHour = Convert.ToDecimal(val.SelectToken("quote.USD.percent_change_1h"));
                    marketCap.ChangeHour = Math.Round(changeHour, 2) + "%";

                    decimal changeDay = Convert.ToDecimal(val.SelectToken("quote.USD.percent_change_24h"));
                    marketCap.ChangeDay = Math.Round(changeDay, 2) + "%";

                    decimal marketCapValue = Convert.ToDecimal(val.SelectToken("quote.USD.market_cap"));
                    marketCap.MarketCapValue = "$" + Math.Round(marketCapValue, 2);

                    var iconUrl = ConfigurationManager.AppSettings.Get("iconUrl");
                    var icon = val.SelectToken("id").ToString() + ".png";
                    iconUrl += icon;
                    marketCap.Icon = iconUrl;

                    string lastUpdated = Convert.ToString(val.SelectToken("quote.USD.last_updated"));
                    marketCap.LastUpdated = lastUpdated;

                    MarketCapList.Add(marketCap);
                }

            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }
            return View("MarketCapList", MarketCapList);
        }

        public string makeAPICall()
        {
            string API_KEY = ConfigurationManager.AppSettings.Get("api_key"); 
            var URL = new UriBuilder(ConfigurationManager.AppSettings.Get("url"));

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = ConfigurationManager.AppSettings.Get("start");  
            queryString["limit"] = ConfigurationManager.AppSettings.Get("limit"); 
            queryString["convert"] = ConfigurationManager.AppSettings.Get("convert");

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            return client.DownloadString(URL.ToString());
        }
    }
}