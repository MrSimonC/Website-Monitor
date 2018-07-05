using System;
using SlackWebAPI;
using static SlackWebAPI.SlackClientAPI;

namespace WebsiteMonitor
{
    class WebsiteMonitor
    {
        static void Main(string[] args)
        {
            // Monitor setup
            string url = @"https://www.sainsburys.co.uk/shop/gb/groceries/instore-bakery-doughnuts-cookies/sainsburys-doughnuts-jam-ball-x5-6543180-p";
            string xpath = "//*[@id=\"addItem_115921\"]/div[1]/p[1]";  // FAIL SINCE SITE REQUIRES JAVASCRIPT TO WORK

            // Get current value
            var wh = new WebHelper();
            try
            {
                string currentValue = wh.GetInnerText(url, xpath).Trim();
                SlackSend($"Hello. Current value is: {currentValue}");
            }
            catch (Exception)
            {
                SlackSend("Error trying to get latest value. Likely website change.");
                Environment.Exit(-1);
            }
            System.Console.WriteLine("Program finished.");
        }

        public static void SlackSend(string message)
        {
            // credentials
            string _channel = SlackCredentials.SlackChannelName;
            string _token = SlackCredentials.SlackToken;

            // send
            var slack = new SlackClientAPI(_token);
            var response = slack.PostMessage("chat.postMessage", new Arguments() {
                Channel = _channel,
                Text = message
            });
            
        }

    }
}
