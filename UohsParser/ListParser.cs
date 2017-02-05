using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace UohsParser
{
    class ListParser
    {
        private const string MAIN_WEB = "http://www.uohs.cz/cs/verejne-zakazky/sbirky-rozhodnuti.html";
        private const string POST_DATA = "jednaci_cislo=&vec=&ucastnik=&text=&rok=2015&rok_vydani=&odbor=2&typ_rozhodnuti=&typ_rizeni=&instance=II.";
        public List<string> DecisionLinks { set; get; }
        private string cookieId;

        public ListParser()
        {
            DecisionLinks = new List<string>();     
        }

        private string DownloadMain()
        {
            var request = (HttpWebRequest)WebRequest.Create(MAIN_WEB);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(POST_DATA);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();

            cookieId = ParseCookie(response.Headers[HttpResponseHeader.SetCookie]);

            response.Close();

            return responseFromServer;
        }

        private void ParsePage(string htmlCode)
        {
            Match match = Regex.Match(htmlCode, "(?<=<td   class=\"al\" ><a   href=\"\\/cs\\/verejne-zakazky\\/sbirky-rozhodnuti\\/)detail-\\d+\\.html");

            while (match.Success)
            {
                DecisionLinks.Add(match.Value);
                match = match.NextMatch();
            }
        }

        private int ParseSize(string htmlCode)
        {
            Match match = Regex.Match(htmlCode, "(?=\\d+\\.html\">poslední)\\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return 100;
        }

        private string DownloadNext(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Uri("http://www.uohs.cz"), new Cookie("PHPSESSID", cookieId));
            var response = (HttpWebResponse)request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        private string ParseCookie(string cookie)
        {
            Match match = Regex.Match(cookie, "(?<=PHPSESSID=)[^;]+");
            return match.Value;
        }

        public void Parse()
        {
            string firstPage = DownloadMain();
            int size = ParseSize(firstPage);
            ParsePage(firstPage);
            //Console.WriteLine($"page 1/{size} decisions {DecisionLinks.Count}");
            for (int i = 2; i <= size; i++)
            {
                string url = $"http://www.uohs.cz/cs/verejne-zakazky/sbirky-rozhodnuti/{i}.html";
                string page = DownloadNext(url);
                ParsePage(page);
                //Console.WriteLine($"page {i}/{size} decisions {DecisionLinks.Count}");
            }
        }


    }
}
