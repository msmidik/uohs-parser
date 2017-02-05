using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UohsParser
{
    class DecisionSecond
    {
        public string Id { set; get; }
        public string CJ { set; get; }
        public string ZeDne { set; get; }
        public string PravniMoc { set; get; }
        public string Vec { set; get; }
        public string RozhodnutoDle { set; get; }
        public string Souvisejici { set; get; }
        public string ZadavatelIco { set; get; }
        public string VZCislo { set; get; }  
        public string RozkladZeDne { set; get; }      

        public override string ToString()
        {
            string s = DecisionParser.SEPARATOR;
            return $"{Id}{s}{CJ}{s}{ZeDne}{s}{PravniMoc}{s}{Vec}{s}{RozhodnutoDle}{s}{Souvisejici}{s}{ZadavatelIco}{s}{VZCislo}{s}{RozkladZeDne}";
        }


    }
}
