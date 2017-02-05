using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UohsParser
{
    class Decision
    {
        public string Id { set; get; }
        public string CJ { set; get; }
        public string CJShort { set; get; }
        public string ZeDne { set; get; }
        public string PravniMoc { set; get; }
        public string Vec { set; get; }
        public string RozhodnutoDle { set; get; }
        public string ZadavatelIco { set; get; }
        public string VZCislo { set; get; }
        public string Pokuta { set; get; }
        public string Naklady { set; get; }
        public string Zahajeno { set; get; }
        public bool? NaNavrh { set; get; }
        public string Typ { set; get; }
        public string ZadavatelZastoupen { set; get; }
        public string NavrhovatelZastoupen { set; get; }

        public override string ToString()
        {
            string s = DecisionParser.SEPARATOR;
            return $"{Id}{s}{CJ}{s}{CJShort}{s}{ZeDne}{s}{PravniMoc}{s}{Vec}{s}{RozhodnutoDle}{s}{ZadavatelIco}{s}{VZCislo}{s}{Pokuta}{s}{Naklady}{s}{Zahajeno}{s}{NaNavrh}{s}{Typ}{s}{ZadavatelZastoupen}{s}{NavrhovatelZastoupen}";
        }
    }
}
