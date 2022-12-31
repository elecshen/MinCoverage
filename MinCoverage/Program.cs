namespace MinCoverage
{
    public class Program
    {

        static void Main(string[] args)
        {
            // Функция заданная интервалами
            // Имеет ядерные строки и циклический остаток
            List<string> input1 = new()
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
            // Массив значений таблицы
            // Имеет ядерные строки, строку-предшественницу, столбец-последователь и циклический остаток
            string[] input2 = new string[]
            {
                "01010100",
                "01100000",
                "10010100",
                "10000011",
                "10010010",
                "00001101",
                "00001011"
            };
            // Массив значений таблицы
            // Имеет ядерные строки и однострочное покрытие
            string[] input3 = new string[]
            {
                "010111",
                "000000",
                "010001",
                "000110",
                "101000",
            };
            CliveTable ct = new(input3);
            List<string> sDNF = new();
            while (true)
            {
                ct.ShowTable();
                if (ct.FindSingleLineCoverage(sDNF))
                    break;
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