using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckerSort
{
    //ソート用の動的配列クラス
    class SortArray<T> where T : IComparable<T>, IComparable<int>
    {
        private T[] _items;
        private T _defo;
        private int _arrayNum;
        private int _seek;
        private readonly int _unit;

        public T this[int index]
        {
            get => _items[index];

            set => _items[index] = value;
        } //インデクサー

        //コンストラクタ
        public SortArray(int unit)
        {
            _unit = unit;
            _defo = default(T);
            _seek = 0;
            _arrayNum = _unit;
            _items = new T[_unit];
        }

        //統合して配列で返す
        public static T[] MergeToArray(SortArray<T>[] arrays)
        {
            int iNum = arrays.Length;
            int totalItem = 1;
            for (int i = 0; i < iNum; i++)
            {
                if (arrays[i] == null)
                    continue;
                totalItem += arrays[i]._seek;
            }
            T[] mergeArray = new T[totalItem];
            int seek = 0;
            for (int i = 0; i < iNum; i++)
            {
                if (arrays[i] == null)
                    continue;
                Array.Copy(arrays[i]._items, 0, mergeArray, seek, arrays[i]._seek);
                seek += arrays[i]._seek;
            }
            mergeArray[seek++] = default(T);
            return mergeArray;
        }

        //配列に追加
        public void Add(T item)
        {
            //値格納
            _items[_seek++] = item;
            //拡張
            if (_seek == _arrayNum)
            {
                T[] arrayBuf = _items;
                _items = new T[_arrayNum + _unit];
                Array.Copy(arrayBuf, 0, _items, 0, _arrayNum);
                _arrayNum += _unit;
            }
        }

        //配列に変更
        public T[] ToArray()
        {
            T[] reItems = new T[_seek];
            Array.Copy(_items, 0, reItems, 0, _seek);
            return reItems;
        }

        //ソート
        public void Sort()
        {
            Array.Sort(_items, 0, _seek);
        }

        //探索
        public int Find(int a)
        {

            for (int i = 0; i < _seek; i++)
            {
                if (_items[i].CompareTo(a) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    //かぶりがある場合に使用する構造体
    struct Suffer : IComparable<Suffer>, IComparable<int>
    {
        public int Value; //被っている部分
        public int Suffers; //被っている数

        public Suffer(int v, int s)
        {
            Value = v;
            Suffers = s;
        }

        public int CompareTo(Suffer a)
        {
            return Value - a.Value;
        }
        public int CompareTo(int a)
        {
            return Value - a;
        }
    }

    //チェッカーソート
    public class CSort
    {
        private int[] _checker; //チェック用の配列
        private SortArray<Suffer>[] _sufSA;//被ったときの
        private int _sufSAnum;//配列数
        private int _sufSAcontainNum;//sufSAの初期化の値
        private int _min;
        private int _max;


        //コンストラクタ　要素の最小値と最大値　おおよその被る数
        public CSort(int min, int max, int paramA = 65536, int paramB = 400)
        {
            _min = min;
            _max = max;

            int mm= checked(_max - _min);

            _checker = new int[(max - min) / 32 + 1];
            _sufSAnum = paramA;
            _sufSAcontainNum = paramB;
            _sufSA = new SortArray<Suffer>[_sufSAnum];

        }

        //ソート開始
        public int StartSort(int[] box)
        {
            int boxNum = box.Length;
            _sufSA = new SortArray<Suffer>[_sufSAnum];//
            _checker = new int[_checker.Length];
            int sufU = (int)((long)_checker.Length * 32 / _sufSAnum) + 1; //被りの配列の単位

            for (int i = 0; i < boxNum; i++)
            {
                int pos = box[i]-_min;//チェックの付ける場所を指定

                int a = pos / 32; //チェッカーのインデックス
                int b = 1 << (pos % 32); //チェッカーの場所
                if ((_checker[a] & b) == 0)//まだチェックをつけていなければ
                {
                    _checker[a] += b;//チェックをつける
                }
                else//すでにチェックがついていたら（被ったら）
                {
                    int sg = pos / sufU;//被り配列のグループ番号
                    if (_sufSA[sg] != null)//すでに被り配列が作られていたら
                    {
                        int sufIn = _sufSA[sg].Find(pos);//すでに被っているか検索
                        if (sufIn == -1)//初めて被ったら
                        {
                            //新しいかぶりを作成
                            _sufSA[sg].Add(new Suffer(pos, 2));
                        }
                        else//すでに被っていたら
                        {
                            //かぶりを追加
                            Suffer suf = new Suffer(pos, _sufSA[sg][sufIn].Suffers + 1);
                            _sufSA[sg][sufIn] = suf;
                        }
                    }
                    else//まだ作られていなければ
                    {
                        //新しい被り配列を作成
                        _sufSA[sg] = new SortArray<Suffer>(_sufSAcontainNum);

                        _sufSA[sg].Add(new Suffer(pos, 2));
                    }
                }
            }

            //被り配列の並び替え
            for (int i = 0; i < _sufSAnum; i++)
            {
                if (_sufSA[i] != null)
                    _sufSA[i].Sort();
            }

            //配列統合
            Suffer[] sufA = SortArray<Suffer>.MergeToArray(_sufSA);

            //仕上げ
            int checkerNum = _checker.Length;
            int I = 0;
            int num = _min;

            int sufI = 0;
            Suffer nextSuf = sufA[sufI++];

            int bs = 0;
            for (int i = 0; i < checkerNum; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if ((_checker[i] & (1 << j)) != 0)
                    {
                        box[I++] = num;
                        if (nextSuf.Suffers != 0 && nextSuf.Value == num)
                        {
                            for (int k = 1; k < nextSuf.Suffers; k++)
                            {

                                box[I++] = num;
                            }
                            nextSuf = sufA[sufI++];
                        }
                    }
                    num++;

                }
            }
            return I;
        }
    }
}
