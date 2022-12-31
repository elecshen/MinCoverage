namespace MinCoverage
{
    public class CliveTable
    {
        public List<string> Intervals;
        public List<string> Points;
        public short[,] Table;

        /// <summary>
        /// Конструктор для непосредственного заполнения таблицы значениями,
        /// названия столбцов и строк заполняются автоматически
        /// </summary>
        /// <param name="valueArr">Массив строк, каждая из которых хранит значения строки таблицы(например, "00101")</param>
        public CliveTable(string[] valueArr)
        {
            FillVectorsAndPoints(valueArr.Length, valueArr[0].Length);
            FillTableFromValues(valueArr);
        }

        /// <summary>
        /// Конструкто заполняющий таблицу по списку интервалов, которые задают функцию
        /// </summary>
        /// <param name="intervals">Список строк, каждая из которых хранит интервал(например, "10-0-")</param>
        public CliveTable(List<string> intervals)
        {
            this.Intervals = intervals;
            AutoFillPoints();
            AutoFillTable();
        }

        // Задает обозначения строк латинскими буквами (А, B, C, D...)
        // Значения столбцов задаются цифрами (1, 2, 3, 4...)
        private void FillVectorsAndPoints(int rows, int columns)
        {
            Intervals = new List<string>();
            Points = new List<string>();
            for (int i = 0; i < rows; i++)
                Intervals.Add(Convert.ToChar('A' + i).ToString());
            for (int i = 0; i < columns; i++)
                Points.Add((i + 1).ToString());
        }

        // Заполняет таблицу из передаваемого списка строк
        private void FillTableFromValues(string[] valueArr)
        {
            Table = new short[valueArr.Length, valueArr[0].Length];
            char[] currentRow;
            for (int r = 0; r < valueArr.Length; r++)
            {
                currentRow = valueArr[r].ToCharArray();
                for (int c = 0; c < currentRow.Length; c++)
                {
                    Table[r, c] = short.Parse(currentRow[c].ToString());
                }
            }
        }

        // Находит все точки которые задаются списком интервалов функции (СДНФ функции)
        private void AutoFillPoints()
        {
            Points = new List<string>();
            foreach (var v in Intervals)
                Points.AddRange(GetPoints(v));
            Points = Points.Distinct().ToList();
            if (Points.Contains(""))
                Points.Remove("");
        }

        // Получает на вход интервал, а возвращает список векторов интервала
        private List<string> GetPoints(string interval)
        {
            if (!interval.Contains('-'))
                return new List<string>() { interval };
            char[] chArr = interval.ToCharArray();
            string interval1 = "", interval2 = "";
            bool isFindDash = false;
            for (int i = 0; i < chArr.Length; i++)
            {
                if (chArr[i] == '-' && !isFindDash)
                {
                    interval2 = interval1;
                    interval1 += '0';
                    interval2 += '1';
                    isFindDash = true;
                }
                else if (isFindDash)
                {
                    interval1 += chArr[i];
                    interval2 += chArr[i];
                }
                else
                    interval1 += chArr[i];
            }
            List<string> outStr = new();
            outStr.AddRange(GetPoints(interval1));
            outStr.AddRange(GetPoints(interval2));
            return outStr;
        }

        // Заполняет таблицу 1 и 0 по спису интервалов и СДНФ. Если на пересечении вектора и интервала,
        // вектор принадлежит интервалу, то записывается 1, в противном случае 0 
        private void AutoFillTable()
        {
            Table = new short[Intervals.Count, Points.Count];
            for (int r = 0; r < Intervals.Count; r++)
            {
                for (int c = 0; c < Points.Count; c++)
                {
                    Table[r, c] = IsInInterval(Intervals[r], Points[c]);
                }
            }
        }

        // Проверяет принадлежит ли вектор интервалу
        // Если принадлежит возвращается 1, в противном случае 0
        private short IsInInterval(string interval, string point)
        {
            var v = interval.ToCharArray();
            var p = point.ToCharArray();
            for (int i = 0; i < interval.Length; i++)
            {
                if (!(v[i] == p[i] || v[i] == '-'))
                    return 0;
            }
            return 1;
        }

        // Выводит таблицу в консоль
        public void ShowTable()
        {
            Console.Write(string.Format("{0, " + Intervals.First().Length + "}", ""));
            string format = "{0, " + (Intervals.First().Length + 1) + "}";
            foreach (var p in Points)
                Console.Write(string.Format(format, p));
            Console.WriteLine();
            for (int r = 0; r < Intervals.Count; r++)
            {
                Console.Write(Intervals[r]);
                for (int c = 0; c < Points.Count; c++)
                {
                    Console.Write(string.Format(format, Table[r, c]));
                }
                Console.WriteLine();
            }
        }

        // Удаляет строку из таблицы по номеру, смещаяя строки таблицы, находящиеся ниже удаляемой, наверх
        // Из списка интервалов удаляется обозначение строки
        public void DeleteRow(int row)
        {
            for(int r = row; r < Intervals.Count - 1; r++)
            {
                for(int c=0;c<Points.Count;c++)
                {
                    Table[r,c] = Table[r+1,c];
                }
            }
            Intervals.RemoveAt(row);
        }

        // Определяет номер строки по содержимому и запускает функицию удаления по номеру
        public void DeleteRow(string interval)
        {
            int num = Intervals.IndexOf(interval);
            DeleteRow(num);
        }

        // Удаляет столбец из таблицы по номеру, смещаяя столбцы таблицы, находящиеся правее удаляемой, влево
        // Из списка точек удаляется обозначение столбца
        public void DeleteColoum(int coloum)
        {
            for (int c = coloum; c < Points.Count - 1; c++)
            {
                for (int r = 0; r < Intervals.Count; r++)
                {
                    Table[r, c] = Table[r, c + 1];
                }
            }
            Points.RemoveAt(coloum);
        }

        // Определяет номер столбца по содержимому и запускает функицию удаления по номеру
        public void DeleteColoum(string coloum)
        {
            int num = Points.IndexOf(coloum);
            DeleteColoum(num);
        }

        // Ищет строку, которая покрывает всю таблицу
        public bool FindSingleLineCoverage(List<string> sDNF)
        {
            for (int r = 0; r < Intervals.Count; r++)
            {
                if (FindCoveringColoums(r).Count == Points.Count)
                {
                    sDNF.Add(Intervals[r]);
                    return true;
                }
            }
            return false;
        }

        // Ищет первый столбец, имеющий только однну строку со значением 1, и возвращает номер этой строки
        private int FindCoreRow()
        {
            int flag = -1;
            for (int c = 0; c < Points.Count; c++)
            {
                for (int r = 0; r < Intervals.Count; r++)
                {
                    if (flag == -1 && Table[r, c] == 1)
                        flag = r;
                    else if (flag != -1 && Table[r, c] == 1)
                    {
                        flag = -1;
                        break;
                    }
                }
                if (flag != -1)
                    return flag;
            }
            return flag;
        }

        // Возвращает список столбцов, которые имеют значение 1 на пересечении с данной строкой
        private List<string> FindCoveringColoums(int row)
        {
            List<string> coloums = new();
            for(int c = 0; c < Points.Count; c++)
            {
                if (Table[row, c] == 1)
                    coloums.Add(Points[c]);
            }
            return coloums;
        }

        // Ищет и затем удаляет строки, которые состоят только из значений 0
        private List<string> FindEmptyRow()
        {
            List<string> emptyRows = new();
            bool isEmpty;
            for(int r = 0; r < Intervals.Count; r++)
            {
                isEmpty = true;
                for (int c = 0; c < Points.Count; c++)
                    if (Table[r, c] == 1)
                    {
                        isEmpty= false;
                        break;
                    }
                if(isEmpty)
                    emptyRows.Add(Intervals[r]);
            }
            return emptyRows;
        }

        // Функция удаляет первую попавшуюся ядерную строку и после очищает пустые строки, если такие есть
        // Обозначения ядерных строк добавляются в получаемый список
        public bool CoreRule(List<string> sDNF)
        {
            bool isSmthDone = false;
            int row;
            while (true)
            {
                row = FindCoreRow();
                if (row != -1)
                {
                    if (!isSmthDone)
                        isSmthDone = true;
                    sDNF.Add(Intervals[row]);
                    foreach (var c in FindCoveringColoums(row))
                    {
                        DeleteColoum(c);
                    }
                    DeleteRow(row);
                }
                else
                    break;
            }
            List<string> emptyRows = FindEmptyRow();
            if (!isSmthDone && emptyRows.Count != 0)
                isSmthDone = true;
            foreach (var r in emptyRows)
                DeleteRow(r);
            return isSmthDone;
        }

        // Сравнивает столбцы таблицы по их номерам
        // Возвращает номер большего столбца, 0 если равны и -1 если не сравнимы
        private int IsComparableColoums(int first, int second)
        {
            int fOs = 0;
            for (int r = 0; r < Intervals.Count; r++)
            {
                if (Table[r, first] != Table[r,second])
                {
                    if (fOs == 0)
                    {
                        if (Table[r, first] > Table[r, second])
                            fOs = 1;
                        else
                            fOs = 2;
                    }
                    else if (fOs != 0)
                    {
                        if ((fOs == 1 && Table[r, first] < Table[r, second]) || (fOs == 2 && Table[r, first] > Table[r, second]))
                            fOs = -1;
                    }
                }
            }
            return fOs;
        }

        // Сравнивает строки таблицы по их номерам
        // Возвращает номер большей строки, 0 если равны и -1 если не сравнимы
        private int IsComparableRows(int first, int second)
        {
            int fOs = 0;
            for (int c = 0; c < Points.Count; c++)
            {
                if (Table[first, c] != Table[second, c])
                {
                    if (fOs == 0)
                    {
                        if (Table[first, c] > Table[second, c])
                            fOs = 1;
                        else
                            fOs = 2;
                    }
                    else if (fOs != 0)
                    {
                        if ((fOs == 1 && Table[first, c] < Table[second, c]) || (fOs == 2 && Table[first, c] > Table[second, c]))
                            fOs = -1;
                    }
                }
            }
            return fOs;
        }

        // Ищет в таблице сравнимые строки и удаляет меньшую из них
        public bool PredRowRule()
        {
            int compareResult;
            foreach (var rFirst in Intervals.ToList())
            {
                if (rFirst == Intervals.Last())
                    break;
                int rFirstNum = Intervals.IndexOf(rFirst);
                foreach (var rSecond in Intervals.GetRange(rFirstNum + 1, Intervals.Count - rFirstNum - 1))
                {
                    int rSecondNum = Intervals.IndexOf(rSecond);
                    compareResult = IsComparableRows(rFirstNum, rSecondNum);
                    if (compareResult == -1)
                        continue;
                    if (compareResult == 1)
                        DeleteRow(rSecondNum);
                    else
                        DeleteRow(rFirstNum);
                    return true;
                }
            }
            return false;
        }

        // Ищет в таблице сравнимые столбцы и удаляет больший из них
        public bool FollColoumRule()
        {
            int compareResult;
            foreach(var cFirst in Points.ToList())
            {
                if (cFirst == Points.Last())
                    break;
                int cFirstNum = Points.IndexOf(cFirst);
                foreach(var cSecond in Points.GetRange(cFirstNum + 1, Points.Count - cFirstNum - 1))
                {
                    int cSecondNum = Points.IndexOf(cSecond);
                    compareResult = IsComparableColoums(cFirstNum, cSecondNum);
                    if (compareResult == -1)
                        continue;
                    if (compareResult == 2)
                        DeleteColoum(cSecondNum);
                    else
                        DeleteColoum(cFirstNum);
                    return true;
                }
            }
            return false;
        }

        // Составляет из обозначений строк таблицы КНФ и преобразует её в ДНФ
        // Элемент наименьшего ранга возвращаяется как список обозначений строк
        public List<string> KNFtoDNF()
        {
            // Составление КНФ
            List<List<List<string>>> NF = new();
            List<List<string>> sum, sum2;
            for(int c = 0; c < Points.Count; c++)
            {
                sum = new();
                for(int r = 0; r < Intervals.Count; r++)
                {
                    if(Table[r, c] == 1)
                        sum.Add(new List<string>() { Intervals[r] });
                }
                NF.Add(sum);
            }

            // Преобразование в ДНФ
            while(NF.Count > 1)
            {
                sum = NF[0];
                sum2 = NF[1];
                NF.RemoveRange(0, 2);
                sum = MultiplyToSum(sum, sum2);
                Unite(sum);
                NF.Add(sum);
            }

            // Поиск элемента ДНФ наименьшего ранга
            sum = NF[0];
            int minLen = int.MaxValue;
            List<string> min = null;
            foreach(var el in sum)
            {
                if(el.Count < minLen)
                {
                    minLen = el.Count;
                    min = el;
                }
            }
            
            return min ?? new List<string>();
        }

        // Преоразует два элемента КНФ в ДНФ получаемую потём перемножения элементов друг на друга
        private List<List<string>> MultiplyToSum(List<List<string>> sum1, List<List<string>> sum2)
        {
            List<List<string>> sum = new();
            foreach(var s1 in sum1)
                foreach(var s2 in sum2)
                {
                    sum.Add(s1.Union(s2).ToList());
                }
            return MakeUniq(sum);
        }

        // Удаляет одинаковые элементы
        private List<List<string>> MakeUniq(List<List<string>> sum)
        {
            return sum.DistinctBy(x => GetStrKey(x)).ToList();
        }

        // Создаёт строку ключ, которая позваляет легче сравнивать списки
        private string GetStrKey(List<string> x)
        {
            x.Sort();
            string key = "";
            foreach (var el in x)
                key += el;
            return key;
        }

        // Делает поглощения элементов
        private void Unite(List<List<string>> sum)
        {
            string key1, key2;
            foreach(var el1 in sum.ToList())
            {
                if (el1 == sum.Last())
                    break;
                int pos1 = sum.IndexOf(el1);
                if (pos1 == -1)
                    continue;
                foreach(var el2 in sum.GetRange(pos1+1, sum.Count - pos1 - 1))
                {
                    key1 = GetStrKey(el1);
                    key2 = GetStrKey(el2);
                    if(key2.All(c => key1.Contains(c)))
                    {
                        sum.Remove(el1);
                        break;
                    }
                    else if(key1.All(c => key2.Contains(c)))
                    {
                        sum.Remove(el2);
                    }
                }
            }
        }
    }
}
