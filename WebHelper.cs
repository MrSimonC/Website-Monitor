using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebsiteMonitor
{
    class WebHelper
    {
        HtmlWeb web = new HtmlWeb();
        
        public string GetInnerText(string url, string xpath)
        {
            var doc = web.Load(url);
            System.Console.WriteLine(doc.Text);
            var txt = doc.DocumentNode.SelectSingleNode(xpath);
            return ""; // HttpUtility.HtmlDecode(txt);
        }
    }
}
