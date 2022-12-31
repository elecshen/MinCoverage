using System;
using System.Xml.Serialization;

namespace MinCoverage
{
    public class Program
    {

        static void Main(string[] args)
        {
            List<string> vectors = new()
            {
                "000-",
                "01-1",
                "110-",
                "10-1",
                "1-10",
                "11-0",
                "101-",
                "--01"
            };
            string[] vectors2 = new string[]
            {
                "01010100",
                "01100000",
                "10010100",
                "10000011",
                "10010010",
                "00001101",
                "00001011"
            };
            CliveTable ct = new(vectors);
            List<string> sDNF = new();
            while (true)
            {
                ct.ShowTable();
                if (ct.CoreRule(sDNF))
                    continue;
                if (ct.PredRowRule())
                    continue;
                if (ct.FollColoumRule())
                    continue;
                sDNF.AddRange(ct.KNFtoDNF());
                break;
            }
            Console.WriteLine("ДНФ:");
            foreach(var el in sDNF)
            {
                Console.Write(el + ", ");
            }
            Console.WriteLine();
        }
    }
}