using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckerSort
{
    //ソート用の動的配列クラス
    class SortArray<T> where T : IComparable<T>, IComparable<int>
    {
        private T[] _items;//要素
        private int _arrayNum;//現在のアイテムの配列数
        private int _seek;//現在の値の数
        private readonly int _unit;

        public int Count => _seek;

        public T this[int index]
        {
            get => _items[index];
        
            set => _items[index] = value;
        } //インデクサー

        //コンストラクタ
        public SortArray(int unit)
        {
            _unit = unit;
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
        public int Position; //被っている部分
        public int Suffers; //被っている数

        public Suffer(int v, int s)
        {
            Position = v;
            Suffers = s;
        }
        public int CompareTo(Suffer a)
        {
            return Position - a.Position;
        }
        public int CompareTo(int a)
        {
            return Position - a;
        }
    }

    //チェッカーソート
    public class CSort
    {
        private int[] _checker; //チェック用の配列
        private SortArray<Suffer>[] _sufA;//被り情報
        private int _sufAnum;//被り情報の配列数
        private int _sufAcontainNum;//sufAのコンストラクタの値
        private int _min;//使用する値の最大値
        private int _max;//使用する値の最小値


        //コンストラクタ　要素の最小値と最大値　
        public CSort(int min, int max, int paramA = 65536, int paramB = 400)
        {
            //要素の最大値と最小値
            _min = min;
            _max = max;

            //チェッカーの初期化
            int checkerNum= checked (_max - _min)/32+1;//値の範囲が合計でint.MaxValueを越えると例外がでる
            _checker = new int[checkerNum];

            //被り情報の初期化
            _sufAnum = paramA;
            _sufAcontainNum = paramB;
            _sufA = new SortArray<Suffer>[_sufAnum+1];

        }

        //ソート開始
        public int StartSort(int[] box)
        {
            //初期化
            int boxNum = box.Length;//要素数の数
            _checker = new int[(_max - _min) / 32 + 1];//チェッカー
            _sufA = new SortArray<Suffer>[_sufAnum];//被り情報
            int sufU = (int)((long)_checker.Length * 32 / _sufAnum) + 1; //被りの配列の単位

            //没
            //_BcheckDiv = (_max - _min) / 3 < boxNum ? 0 : (_max - _min) / boxNum;
            //_Bchecker =new int[(_max - _min)/32/_BcheckDiv+1];

            int maxSufAI = 0;//SufAの使用している最大インデックス


            //初期化と同時に最初の要素を調べる
            int pos= box[0] - _min;//チェックポイントの場所

            int maxPos = pos;//最大値のチェックポイント
            int minPos = pos;//最小値のチェックポイント

            _checker[pos / 32] += 1 << (pos % 32);


            for (int i = 1; i < boxNum; i++)
            {
                pos = box[i]-_min;//チェックの付ける場所を指定

                int a = pos / 32; //チェッカーのインデックス
                int b = 1 << (pos % 32); //チェッカーの場所
                if ((_checker[a] & b) == 0)//まだチェックをつけていなければ
                {
                    if (maxPos < pos)
                        maxPos = pos;
                    if (minPos > pos)
                        minPos = pos;
                        
                    //_Bchecker[a / _BcheckDiv] = _Bchecker[a / _BcheckDiv] | (1 << (pos / _BcheckDiv % 32));没

                    _checker[a] += b;//チェックをつける
                }
                else//すでにチェックがついていたら（被ったら）
                {
                    int sg = pos / sufU;//被り配列のグループ番号
                    if (_sufA[sg] != null)//すでに被り配列が作られていたら
                    {
                        int sufIn = _sufA[sg].Find(pos);//すでに被っているか検索
                        if (sufIn == -1)//初めて被ったら
                        {
                            //新しいかぶりを作成
                            _sufA[sg].Add(new Suffer(pos, 2));
                        }
                        else//すでに被っていたら
                        {
                            //かぶりを追加
                            Suffer suf = new Suffer(pos, _sufA[sg][sufIn].Suffers + 1);
                            _sufA[sg][sufIn] = suf;
                        }
                    }
                    else//まだ作られていなければ
                    {
                        if (maxSufAI < sg)
                            maxSufAI=sg;
                        //新しい被り配列を作成
                        _sufA[sg] = new SortArray<Suffer>(_sufAcontainNum);

                        _sufA[sg].Add(new Suffer(pos, 2));
                    }
                }
            }

            //被り配列の並び替え
            for (int i = 0; i < _sufAnum; i++)
            {
                if (_sufA[i] != null)
                    _sufA[i].Sort();
            }

            //配列統合（旧_sufSAはそのまま使用することにした）
            //Suffer[] sufA = SortArray<Suffer>.MergeToArray(_sufSA);

            //仕上げ
            int checkerNum = maxPos/32+1;//使用するチェッカーの数
            int I = 0;

            //pos = minPos / 32 * 32;　加算処理の削減

            int sufI = 0;
            int sufAI = 0;

            //OutOfIndexExceptionの対策
            _sufA[maxSufAI + 1]=new SortArray<Suffer>(3);
            _sufA[maxSufAI + 1].Add(new Suffer(0, 0));
            _sufA[maxSufAI + 1].Add(new Suffer(0, 0));

            SortArray<Suffer> nextSufArray;//SufAの要素のバッファ
            while ((nextSufArray = _sufA[sufAI++])==null) ;//null以外の
            Suffer nextSuf = nextSufArray[sufI++];//
             

            int check = 0;//チェッカーのバッファ
            for (int i = minPos/32; i < checkerNum; i++)//初期値を最小値に合わせる
            {
                check = _checker[i];
                if (check != 0)//必要のない箇所は飛ばす
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((check & (1 << j)) != 0)
                        {
                            pos  = i * 32 + j;//posの位置

                            if (nextSuf.Position == pos)//被っている場合
                            {
                                pos += _min;//実際の値だが使いまわす
                                for (int k = 0; k < nextSuf.Suffers; k++)//被った回数だけboxに入れる
                                {
                                    box[I++] = pos;
                                }

                                //次のかぶり要素の準備
                                nextSuf = nextSufArray[sufI++];
                                if (nextSufArray.Count == sufI)
                                {
                                    while((nextSufArray = _sufA[sufAI++])==null);
                                    sufI = 0;
                                }
                            }
                            else//被ってなければ
                            {
                                box[I++] = pos + _min;
                            }
                        }
                    }
                }
            }
            
            /*没
            pos = minPos / _BcheckDiv*_BcheckDiv;
            int BcheckNum = maxPos / _BcheckDiv;
            for (int i = 0; i < BcheckNum; i++)
            {
                if ((_Bchecker[i / 32] & (1 << (i % 32)))!=0)
                {
                    for (int j = 0; j < _BcheckDiv; j++)
                    {
                        if ((_checker[pos/32] & (1 << (pos%32))) != 0)
                        {
                            box[I++] = pos + _min;
                            if (nextSuf.Suffers != 0 && nextSuf.Position == pos)
                            {
                                for (int k = 1; k < nextSuf.Suffers; k++)
                                {
                                    box[I++] = pos + _min;
                                }
                                nextSuf = sufA[sufI++];
                            }
                        }
                        pos++;
                    }
                }
                else
                {
                    pos += _BcheckDiv;
                }
            }
            checkerNum = maxPos / 32 + 1;
            for (int j = 0; j < _BcheckDiv; j++)
            {
                if (0 < pos || pos <= maxPos)
                    break;
                if ((_checker[pos / 32] & (1 << (pos % 32))) != 0)
                {
                    box[I++] = pos + _min;
                    if (nextSuf.Suffers != 0 && nextSuf.Position == pos)
                    {
                        for (int k = 1; k < nextSuf.Suffers; k++)
                        {
                            box[I++] = pos + _min;
                        }
                        nextSuf = sufA[sufI++];
                    }
                }
                pos++;
            }
            */

            return I;
        }
    }
}
