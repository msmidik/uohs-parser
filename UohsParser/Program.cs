using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UohsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var listParser = new ListParser();
            listParser.Parse();
            var fileNames = listParser.DecisionLinks;
            

            
            foreach (string address in  fileNames)
            {
                Console.WriteLine(address);
            }
           // Console.Read();
            
            */

            var decisionParser = new DecisionParser();
            //decisionParser.SaveDecisions(fileNames);

            /*
            //test
            var sr = new StreamReader(@"decisionssecond\detail-12938.html");
            string page = sr.ReadToEnd();
            var decision = decisionParser.ParseDecisionSecond(page);
            Console.WriteLine(decision);
            

            Console.WriteLine("finished");
            Console.Read();
            */
            decisionParser.ParseDecisionsFromFolder("decisions");
            decisionParser.DecisionsToCsv("decisions_zastoupeni.csv");
         //   decisionParser.ParseDecisionsSecondFromFolder("decisionssecond");
        //    decisionParser.DecisionsSecondToCsv("decisionssecond.csv");

            Console.WriteLine("finished");
            Console.Read();
        }
    }
}
