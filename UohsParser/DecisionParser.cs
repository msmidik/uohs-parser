using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UohsParser
{
    class DecisionParser
    {
        private const string DOWNLOAD_FOLDER = "decisionssecond";
        private const string ONLINE_FOLDER = "http://www.uohs.cz/cs/verejne-zakazky/sbirky-rozhodnuti/";
        private const int PAGE_SIZE_LIMIT = 100000;
        private const int PAGE_BOTTOM_LENGHTH = 20000;
        private const int PAGE_BOTTOM_TO_END = 40000;
        public List<Decision> Decisions {set; get;}
        public List<DecisionSecond> DecisionsSecond { set; get; }
        public const string SEPARATOR = "|";

        public DecisionParser()
        {
            Decisions = new List<Decision>();
            DecisionsSecond = new List<DecisionSecond>();
        }

        public void ParseDecisionsOnline(List<string> decisionLinks)
        {
            foreach (string fileName in decisionLinks)
            {
                string page = DownloadFile(ONLINE_FOLDER + fileName);
                var decision = ParseDecision(page);
                decision.Id = fileName;
                Decisions.Add(decision);
            }
        }

        public void ParseDecisionsFromFolder(string folder)
        {
            var files = Directory.GetFiles(folder).ToList();
            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    string page = sr.ReadToEnd();
                    var decision = ParseDecision(page);
                    decision.Id = file;
                    Decisions.Add(decision);
                }
            }
        }       

        public void ParseDecisionsSecondFromFolder(string folder)
        {
            var files = Directory.GetFiles(folder).ToList();
            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    string page = sr.ReadToEnd();
                    var decision = ParseDecisionSecond(page);
                    decision.Id = file;
                    DecisionsSecond.Add(decision);
                }
            }
        }

        public void DecisionsToCsv(string output)
        {
            using (var sw = new StreamWriter(output))
            {
                foreach (var d in Decisions)
                {
                    sw.WriteLine(d.ToString());
                }
            }

        }

        public void DecisionsSecondToCsv(string output)
        {
            using (var sw = new StreamWriter(output))
            {
                foreach (var d in DecisionsSecond)
                {
                    sw.WriteLine(d.ToString());
                }
            }
        }

        public Decision ParseDecision(string page)
        {
            string top = AdjustPage(page)[0];
            string bottom = AdjustPage(page)[1];
            var decision = new Decision();

            // CJ
            var match = Regex.Match(top, @"(?<=<h1   >Rozhodnutí: )Ú?O?H?S?\/?[^\/]+\/\d{4}\/VZ-\d+\/\d{4}\/\d{3}\/?\w{0,5}");
            if (match.Success)
            {
                decision.CJ = match.Value;
            }

            /*
            // CJShort
            if (decision.CJ != null)
            {
                match = Regex.Match(decision.CJ, @"[sS]\d{2,4}\/\d{4}");
                if (match.Success)
                {
                    decision.CJShort = match.Value;
                }
            }

            // ze dne
            match = Regex.Match(top, @"<p\s?>(\d{1,2}\.\s?\S+\s?\d{4})<");
            if (match.Success)
            {
                decision.ZeDne = match.Groups[1].Value;
            }
            else
            {
                match = Regex.Match(top, @"<td>\D*?(\d{1,2}\.\s?\S+\s?\d{4})\s*<\/td>");
                if (match.Success)
                {
                    decision.ZeDne = match.Groups[1].Value;
                }
                else
                {
                    match = Regex.Match(top, @"dokument ke stažení.+?(\d{1,2}\.\s?\S+\s?20\d{2})");
                    if (match.Success)
                    {
                        decision.ZeDne = match.Groups[1].Value;
                    }
                }
            }
           

            // pravni moc
            match = Regex.Match(top, @"<td   >(\d{1,2}\.\s?\d{1,2}\.\s?\d{4})</td>");
            if (match.Success)
            {
                decision.PravniMoc = match.Groups[1].Value;
            }

            // vec
            match = Regex.Match(top, @"<th  >Věc<\/th>\s*<td   >([^<]+)<\/td>");
            if (match.Success)
            {
                decision.Vec = match.Groups[1].Value;
            }

            //rozhodnuto dle
            match = Regex.Match(top, @"<th  >Typ rozhodnutí<\/th>\s*<td   >([^<]+)<\/td>");
            if (match.Success)
            {
                decision.RozhodnutoDle = match.Groups[1].Value;
            }

            //zadavatel ico
            match = Regex.Match(top, @"zadavatele?m?\s?[-–][^<]+?,\s*IČO?:?\s(\d{8}),\sse sídlem");
            if (match.Success)
            {
                decision.ZadavatelIco = match.Groups[1].Value;
            }

            //vz cislo
            match = Regex.Match(top, @"pod ev\.\s?č\.\s?(\d{6,})");
            if (match.Success)
            {
                decision.VZCislo = match.Groups[1].Value;
            }
            else
            {
                match = Regex.Match(top, @"pod evidenčním číslem zakázky (\d{6,})");
                if (match.Success)
                {
                    decision.VZCislo = match.Groups[1].Value;
                }
            }

            // pokuta
            match = Regex.Match(top, @"pokuta ve výši\D+?(\d[^K]+\d),?[-–]?\sKč");
            if (match.Success)
            {
                decision.Pokuta = match.Groups[1].Value;
            }

            // naklady
            match = Regex.Match(top, @"náklady řízení ve výši\D+?(\d[^K]+\d),?[-–]?\sKč");
            if (match.Success)
            {
                decision.Naklady = match.Groups[1].Value;
            }
            */

            // zahajeno
            match = Regex.Match(top, @"ve správním řízení zahájeném (na návrh )?dne (\d{1,2}\.\s?\d{1,2}\.\s?\d{4})(?! z moci)");
            if (match.Success)
            {
                decision.Zahajeno = match.Groups[2].Value;
                decision.NaNavrh = true;
            }
            else
            {
                match = Regex.Match(top, @"zahájeném (z moci úřední )?dne (\d{1,2}\.\s?\d{1,2}\.\s?\d{4})(?! na návrh)");
                if (match.Success)
                {
                    decision.Zahajeno = match.Groups[1].Value;
                    decision.NaNavrh = false;
                }
            }

            /*
            //typ
            match = Regex.Match(bottom, @"Proti tomuto (\D{7,10}) lze");
            if (match.Success)
            {
                decision.Typ = match.Groups[1].Value;
            }

    */
            //zadavatelzastoupen
            match = Regex.Match(bottom, @"zadavatel\s?[-–]\s?[^<]{20,300}ve\s+správním\s+řízení\s+zastoupen.{0,50}?\s\d{1,2}\.\s?\d{1,2}\.\s?20\d{2}\s(.{50})");
            if (match.Success)
            {
                decision.ZadavatelZastoupen = match.Groups[1].Value;
            }

            //navrhovatelzastoupen
            match = Regex.Match(bottom, @"navrhovatel\s?[-–]\s?[^<]{20,300}ve\s+správním\s+řízení\s+zastoupen.{0,50}?\s\d{1,2}\.\s?\d{1,2}\.\s?20\d{2}\s(.{50})");
            if (match.Success)
            {
                decision.NavrhovatelZastoupen = match.Groups[1].Value;
            }

            return decision;
        }

        public DecisionSecond ParseDecisionSecond(string page)
        {
            page = AdjustPage(page)[0];
            var decision = new DecisionSecond();

            // CJ
            var match = Regex.Match(page, @"(?<=<h1   >Rozhodnutí: )Ú?O?H?S?\/?[^\/]+\/\d{4}\/VZ-\d+\/\d{4}\/\d{3}\/?\w{0,5}");
            if (match.Success)
            {
                decision.CJ = match.Value;
            }

            // ze dne
            match = Regex.Match(page, @"<p\s?>(\d{1,2}\.\s?\S+\s?\d{4})<");
            if (match.Success)
            {
                decision.ZeDne = match.Groups[1].Value;
            }
            else
            {
                match = Regex.Match(page, @"<td>\D*?(\d{1,2}\.\s?\S+\s?\d{4})\s*<\/td>");
                if (match.Success)
                {
                    decision.ZeDne = match.Groups[1].Value;
                }
                else
                {
                    match = Regex.Match(page, @"dokument ke stažení.+?(\d{1,2}\.\s?\S+\s?20\d{2})");
                    if (match.Success)
                    {
                        decision.ZeDne = match.Groups[1].Value;
                    }
                }
            }

            // pravni moc
            match = Regex.Match(page, @"<td   >(\d{1,2}\.\s?\d{1,2}\.\s?\d{4})</td>");
            if (match.Success)
            {
                decision.PravniMoc = match.Groups[1].Value;
            }

            // vec
            match = Regex.Match(page, @"<th  >Věc<\/th>\s*<td   >([^<]+)<\/td>");
            if (match.Success)
            {
                decision.Vec = match.Groups[1].Value;
            }

            //rozhodnuto dle
            match = Regex.Match(page, @"<th  >Typ rozhodnutí<\/th>\s*<td   >([^<]+)<\/td>");
            if (match.Success)
            {
                decision.RozhodnutoDle = match.Groups[1].Value;
            }

            //zadavatel ico
            match = Regex.Match(page, @"zadavatele?m?\s?[-–][^<]+?,\s*IČO?:?\s(\d{8}),\sse sídlem");
            if (match.Success)
            {
                decision.ZadavatelIco = match.Groups[1].Value;
            }

            //souvisejici
            match = Regex.Match(page, @"Související rozhodnutí<\/th>\s*<td\s*><a href=.\/cs\/verejne-zakazky\/sbirky-rozhodnuti\/detail-\d+.html.\s*>([sS]\d{2,4}\/\d{2,4})<\/a>");
            if (match.Success)
            {
                decision.Souvisejici = match.Groups[1].Value;
            }

            //vz cislo
            match = Regex.Match(page, @"pod ev\.\s?č\.\s?(\d{6,})");
            if (match.Success)
            {
                decision.VZCislo = match.Groups[1].Value;
            }
            else
            {
                match = Regex.Match(page, @"pod evidenčním číslem zakázky (\d{6,})");
                if (match.Success)
                {
                    decision.VZCislo = match.Groups[1].Value;
                }
            }

            //rozlad z
            match = Regex.Match(page, @"řízení o rozkladu ze dne (\d{1,2}\.\s?\d{1,2}\.\s?\d{4})");
            if (match.Success)
            {
                decision.RozkladZeDne = match.Groups[1].Value;
            }


            return decision;
        }

        public void SaveDecisions(List<string> decisionLinks)
        {
            //int size = decisions.Count;
            Directory.CreateDirectory(DOWNLOAD_FOLDER);
            //int i = 1;
            foreach (string fileName in decisionLinks)
            {
                string file = DownloadFile(ONLINE_FOLDER + fileName);
                File.WriteAllText(DOWNLOAD_FOLDER + @"\" + fileName, file);
                //Console.WriteLine($"{i}/{size} {fileName}");
                //i++;
            }

        }

        private string DownloadFile(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public string[] AdjustPage(string page)
        {
            int pageSize = page.Count();
            int length = Math.Min(pageSize, PAGE_SIZE_LIMIT);
            int lengthBottom = Math.Min(pageSize, PAGE_BOTTOM_LENGHTH);
            int startBottom = Math.Max(pageSize - PAGE_BOTTOM_LENGHTH - PAGE_BOTTOM_TO_END, 0);
            string subpageTop = page.Substring(0, length);
            string subpageBottom = page.Substring(startBottom, lengthBottom);
            var adjustedPage = new string[2];
            adjustedPage[0] = RemoveEndLineAndCsvSeparator(System.Net.WebUtility.HtmlDecode(subpageTop));
            adjustedPage[1] = RemoveEndLineAndCsvSeparator(System.Net.WebUtility.HtmlDecode(subpageBottom));
            return adjustedPage;
        }

        private string RemoveEndLineAndCsvSeparator(string text)
        {
            return text
                .Replace(SEPARATOR, "/")
                .Replace("\n", String.Empty)
                .Replace("\t", String.Empty)
                .Replace("\r", String.Empty);
        }

    }
}
