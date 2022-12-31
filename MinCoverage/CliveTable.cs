using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinCoverage
{
    public class CliveTable
    {
        public List<string> Vectors;
        public List<string> Points;
        public short[,] Table;

        public CliveTable(string[] valueArr)
        {
            FillVectorsAndPoints(valueArr.Length, valueArr[0].Length);
            FillTableFromValues(valueArr);
        }

        public CliveTable(List<string> vectors)
        {
            this.Vectors = vectors;
            AutoFillPoints();
            AutoFillTable();
        }

        private void FillVectorsAndPoints(int rows, int columns)
        {
            Vectors = new List<string>();
            Points = new List<string>();
            for (int i = 0; i < rows; i++)
                Vectors.Add(Convert.ToChar('A' + i).ToString());
            for (int i = 0; i < columns; i++)
                Points.Add((i + 1).ToString());
        }

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

        private void AutoFillPoints()
        {
            Points = new List<string>();
            foreach (var v in Vectors)
                Points.AddRange(GetPoints(v));
            Points = Points.Distinct().ToList();
            if (Points.Contains(""))
                Points.Remove("");
        }

        private List<string> GetPoints(string vector)
        {
            if (!vector.Contains('-'))
                return new List<string>() { vector };
            char[] chArr = vector.ToCharArray();
            string vector1 = "", vector2 = "";
            bool isFindDash = false;
            for (int i = 0; i < chArr.Length; i++)
            {
                if (chArr[i] == '-' && !isFindDash)
                {
                    vector2 = vector1;
                    vector1 += '0';
                    vector2 += '1';
                    isFindDash = true;
                }
                else if (isFindDash)
                {
                    vector1 += chArr[i];
                    vector2 += chArr[i];
                }
                else
                    vector1 += chArr[i];
            }
            List<string> outStr = new();
            outStr.AddRange(GetPoints(vector1));
            outStr.AddRange(GetPoints(vector2));
            return outStr;
        }

        private void AutoFillTable()
        {
            Table = new short[Vectors.Count, Points.Count];
            for (int r = 0; r < Vectors.Count; r++)
            {
                for (int c = 0; c < Points.Count; c++)
                {
                    Table[r, c] = IsInVector(Vectors[r], Points[c]);
                }
            }
        }

        private short IsInVector(string vector, string point)
        {
            var v = vector.ToCharArray();
            var p = point.ToCharArray();
            for (int i = 0; i < vector.Length; i++)
            {
                if (!(v[i] == p[i] || v[i] == '-'))
                    return 0;
            }
            return 1;
        }

        public void ShowTable()
        {
            Console.Write(String.Format("{0, " + Vectors.First().Length + "}", ""));
            string format = "{0, " + (Vectors.First().Length + 1) + "}";
            foreach (var p in Points)
                Console.Write(String.Format(format, p));
            Console.WriteLine();
            for (int r = 0; r < Vectors.Count; r++)
            {
                Console.Write(Vectors[r]);
                for (int c = 0; c < Points.Count; c++)
                {
                    Console.Write(String.Format(format, Table[r, c]));
                }
                Console.WriteLine();
            }
        }

        public void DeleteRow(int row)
        {
            for(int r = row; r < Vectors.Count - 1; r++)
            {
                for(int c=0;c<Points.Count;c++)
                {
                    Table[r,c] = Table[r+1,c];
                }
            }
            Vectors.RemoveAt(row);
        }

        public void DeleteRow(string vector)
        {
            int num = Vectors.IndexOf(vector);
            DeleteRow(num);
        }

        public void DeleteColoum(int coloum)
        {
            for (int c = coloum; c < Points.Count - 1; c++)
            {
                for (int r = 0; r < Vectors.Count; r++)
                {
                    Table[r, c] = Table[r, c + 1];
                }
            }
            Points.RemoveAt(coloum);
        }

        public void DeleteColoum(string coloum)
        {
            int num = Points.IndexOf(coloum);
            DeleteColoum(num);
        }

        private int FindCoreRow()
        {
            int flag = -1;
            for (int c = 0; c < Points.Count; c++)
            {
                for (int r = 0; r < Vectors.Count; r++)
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

        private List<string> FindEmptyRow()
        {
            List<string> emptyRows = new();
            bool isEmpty;
            for(int r = 0; r < Vectors.Count; r++)
            {
                isEmpty = true;
                for (int c = 0; c < Points.Count; c++)
                    if (Table[r, c] == 1)
                    {
                        isEmpty= false;
                        break;
                    }
                if(isEmpty)
                    emptyRows.Add(Vectors[r]);
            }
            return emptyRows;
        }

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
                    sDNF.Add(Vectors[row]);
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

        // Возвращает номер большего, 0 если равны и -1 если не сравнимы
        private int IsComparableColoums(int first, int second)
        {
            int fOs = 0;
            for (int r = 0; r < Vectors.Count; r++)
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

        public bool PredRowRule()
        {
            int compareResult;
            foreach (var rFirst in Vectors.ToList())
            {
                if (rFirst == Vectors.Last())
                    break;
                int rFirstNum = Vectors.IndexOf(rFirst);
                foreach (var rSecond in Vectors.GetRange(rFirstNum + 1, Vectors.Count - rFirstNum - 1))
                {
                    int rSecondNum = Vectors.IndexOf(rSecond);
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

        public List<string> KNFtoDNF()
        {
            List<List<List<string>>> NF = new();
            List<List<string>> sum, sum2;
            for(int c = 0; c < Points.Count; c++)
            {
                sum = new();
                for(int r = 0; r < Vectors.Count; r++)
                {
                    if(Table[r, c] == 1)
                        sum.Add(new List<string>() { Vectors[r] });
                }
                NF.Add(sum);
            }


            while(NF.Count > 1)
            {
                sum = NF[0];
                sum2 = NF[1];
                NF.RemoveRange(0, 2);
                sum = MultiplyToSum(sum, sum2);
                Unite(sum);
                NF.Add(sum);
            }

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

        private List<List<string>> MakeUniq(List<List<string>> sum)
        {
            return sum.DistinctBy(x => GetStrKey(x)).ToList();
        }

        private string GetStrKey(List<string> x)
        {
            x.Sort();
            string key = "";
            foreach (var el in x)
                key += el;
            return key;
        }

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
