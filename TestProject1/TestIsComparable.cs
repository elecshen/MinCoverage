using MinCoverage;

namespace TestProject1
{
    [TestClass]
    public class TestIsComparable
    {
        [TestMethod]
        public void isComparable_1and0_1()
        {
            int expected = 1;
            List<bool> f = new() { true };
            List<bool> s = new() { false };
            Assert.AreEqual(expected, Program.isComparable(f, s));
        }

        [TestMethod]
        public void isComparable_10101and10001_1()
        {
            int expected = 1;
            List<bool> f = new() { true, false, true, false, true };
            List<bool> s = new() { true, false, false, false, true };
            Assert.AreEqual(expected, Program.isComparable(f, s));
        }

        [TestMethod]
        public void isComparable_10101and11001_minus1()
        {
            int expected = -1;
            List<bool> f = new() { true, false, true, false, true };
            List<bool> s = new() { true, true, false, false, true };
            Assert.AreEqual(expected, Program.isComparable(f, s));
        }

        [TestMethod]
        public void isComparable_00001and11001_2()
        {
            int expected = 2;
            List<bool> f = new() { false, false, false, false, true };
            List<bool> s = new() { true, true, false, false, true };
            Assert.AreEqual(expected, Program.isComparable(f, s));
        }

        [TestMethod]
        public void isComparable_10101and10101_0()
        {
            int expected = 0;
            List<bool> f = new() { true, false, true, false, true };
            List<bool> s = new() { true, false, true, false, true };
            Assert.AreEqual(expected, Program.isComparable(f, s));
        }
    }

    [TestClass]
    public class CTDeleteRow
    {
        [TestMethod]
        public void DeleteRow__2rows_like_10001__1row_like10001()
        {
            short[,] expectedTable = new short[2, 5]
            {
                {1,0,0,0,1 },
                {1,0,0,0,1 }
            };
            CliveTable ct = new(new string[] {"10001", "10001"});
            ct.DeleteRow(0);
            bool expected = true;
            for(int r =0; r < expectedTable.GetLength(0); r++)
            {
                for(int c=0; c < expectedTable.GetLength(1); c++)
                {
                    if (expectedTable[r,c] != ct.Table[r, c])
                    {
                        expected = false;
                        break;
                    }
                }
            }
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void DeleteRow__2rows_like_10101_and_10001__1row_like10001()
        {
            short[,] expectedTable = new short[2, 5]
            {
                {1,0,0,0,1 },
                {1,0,0,0,1 }
            };
            CliveTable ct = new(new string[] { "10101", "10001" });
            ct.DeleteRow(0);
            bool expected = true;
            for (int r = 0; r < expectedTable.GetLength(0); r++)
            {
                for (int c = 0; c < expectedTable.GetLength(1); c++)
                {
                    if (expectedTable[r, c] != ct.Table[r, c])
                    {
                        expected = false;
                        break;
                    }
                }
            }
            Assert.IsTrue(expected);
        }
    }
}