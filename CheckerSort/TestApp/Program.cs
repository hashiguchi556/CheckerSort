using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckerSort;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            SortTimer(int.MinValue/2,int.MaxValue/2,100000000);
            SortTimer(int.MinValue, int.MaxValue, 100000000);

            //測定終了
            Console.Write("PressKey...");
            Console.ReadKey();
            Console.WriteLine();
        }

        static void SortTimer(int valueMin,int valueMax,int itemNum)
        {

            System.Diagnostics.Stopwatch sw = new Stopwatch();//ストップウォッチ
            Random rand = new Random();//乱数

            int[] a = new int[itemNum];//作成用の要素
            int[] b = new int[itemNum];//aのコピー
            int[] c = new int[itemNum];//aのコピー

            //要素の作成
            for (int i = 0; i < itemNum; i++)
            {
                a[i] = b[i] = c[i] = rand.Next(valueMin, valueMax);
            }

            Console.WriteLine("要素数 :" + itemNum);
            Console.WriteLine("範囲 :int型" + valueMin + "～" + valueMax);
            Console.WriteLine();

            #region チェッカーソート（固定）
            /*
            Console.WriteLine();
            Console.Write("チェッカーソートv3(固定)...");


            sw.Start();
            //CSort sort = new CSort(valueMin, valueMax);
            //sort.StartSort(a);
            CSort.UnsafeSort(a);
            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            sw.Reset();
            */
            #endregion


            //チェッカーソート
            Console.Write("チェッカーソートv3...");


            sw.Start();
            CSort.Sort(b);
            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            sw.Reset();

            //デフォルトのソート
            Console.Write("デフォルトのソート...");

            sw.Start();

            Array.Sort(c);//デフォルトのソート

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine();
        }
    }
}