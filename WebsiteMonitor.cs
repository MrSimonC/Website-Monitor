using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using SlackWebAPI;
using static SlackWebAPI.SlackClientAPI;


namespace WebsiteMonitor
{
    class WebsiteMonitor
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine($"Working dir is: {AssemblyDirectory}");

            string _url = @"https://www.sainsburys.co.uk/shop/gb/groceries/instore-bakery-doughnuts-cookies/sainsburys-doughnuts-jam-ball-x5-6543180-p";
            string _class = "pricePerUnit";
            string _pricefile = Path.Combine(AssemblyDirectory, "oldvalue.txt");

            IWebDriver driver = new PhantomJSDriver(AssemblyDirectory);
            decimal price = 0;
            try
            {
                // get price
                driver.Navigate().GoToUrl(_url);
                IWebElement priceclass = driver.FindElement(By.ClassName(_class));
                decimal.TryParse(priceclass.Text.Replace("£", "").Replace("/unit", ""), out price);
                driver.Quit();
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Couldn't get price");
                throw;
            }
            finally
            {
                driver.Quit();
            }

            // compare old with new value
            decimal oldPrice = 0;
            try
            {
                decimal.TryParse(File.ReadAllText(_pricefile), out oldPrice);
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Couldn't find previous price / file.");
            }

            if (oldPrice != 0 && price != 0 && oldPrice > price)
            {
                SlackSend($"<@UB1H7F64C> Sainsburys jam :doughnut: price has dropped by {oldPrice - price:C2}. Online it's {price:C2}, which means it's likely {price * 1.1m:C2} in Sainsburys local. Win.");
            }
            else if (oldPrice != 0 && price != 0 && oldPrice < price)
            {
                SlackSend($"Sainsburys jam :doughnut: price has increased by {price - oldPrice:C2}. Online it's gone up to {price:C2} (likely {price * 1.1m:C2} in Sainsburys local). Cruel, just cruel.");
            }

            // save new price value
            if (price != 0)
            {
                try
                {
                    File.WriteAllText(_pricefile, price.ToString());   
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Couldn't write current price to file");
                    throw;
                }
            }
        }

        public static void SlackSend(string message, bool simonOnly=false)
        {
            // credentials
            string _channel;
            string _token;
            
            if (simonOnly)
            {
                _channel = SlackCredentials.ChannelSimon;
                _token = SlackCredentials.TokenSimonLegacy;
            }
            else
            {
                _channel = SlackCredentials.ChannelTiffinTime;
                _token = SlackCredentials.TokenLocalMenuApp;
            }

            // send
            var slack = new SlackClientAPI(_token);
            var response = slack.PostMessage("chat.postMessage", new Arguments() {
                Channel = _channel,
                Text = message
            });
            if (!response.Ok)
            {
                throw new ArgumentException($"Slack error: {response.Error}");
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
