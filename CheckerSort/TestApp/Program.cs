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

            int valueMin = int.MinValue/2;//要素の最小値
            int valueMax = int.MaxValue/2;//要素の最大値
            int itemNum = 100000000;//要素数

            System.Diagnostics.Stopwatch sw = new Stopwatch();//ストップウォッチ
            Random rand = new Random();//乱数

            int[] a = new int[itemNum];//作成用の要素
            int[] b = new int[itemNum];//aのコピー

            //要素の作成
            for (int i = 0; i < itemNum; i++)
            {
                a[i] = rand.Next(valueMin, valueMax);
                b[i] = a[i];
            }

            Console.WriteLine("要素数 :" + itemNum);
            Console.WriteLine("範囲 :int型" + valueMin + "～" + valueMax);


            //チェッカーソート
            Console.WriteLine();
            Console.Write("チェッカーソートv2...");


            sw.Start();
            CSort sort = new CSort(valueMin, valueMax);
            sort.StartSort(a);

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            sw.Reset();

            //デフォルトのソート
            Console.Write("デフォルトのソート...");

            sw.Start();

            Array.Sort(b);//デフォルトのソート

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            //測定終了
            Console.Write("PressKey...");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}