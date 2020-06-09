using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace MZQStringRuleMining
{
    public class ItemSet
    {
        private string items;
        private int sup;

        public string Items
        {
            get { return items; }
            set { items = value; }
        }
        public int Sup
        {
            get { return sup; }
            set { sup = value; }
        }
        public ItemSet()//对象初始化  
        {
            items = null;
            sup = 0;
        }
    }  

    //Apriori算法实现 改造事务分隔方式，将原来的以“,”号为分隔符变为单字符分隔。
    class AprioriAlgorithm
    {
        #region-----用Apriori算法进行迭代-----
        /// <summary>  
        /// 用Apriori算法进行迭代  
        /// </summary>  
        /// <param name="D">字符序列集</param>  
        /// <param name="I">候选序列集</param>  
        /// <param name="sup"></param>  
        /// <returns></returns>  
        
        public static ArrayList C = new ArrayList();//频繁字符集
        public static void setC(ArrayList D, ArrayList I, float sup)
        {
            List<ItemSet> L = new List<ItemSet>();//所有频繁序列集  
            if (I.Count == 0) return;
            else
            {
                int[] Icount = new int[I.Count];//初始序列集计数器,初始化为0  
                ArrayList Ifrequent = new ArrayList();//初始序列集中的频繁序列集  

                //遍历序列集，对候选序列进行计数  
                for (int i = 0; i < D.Count; i++)
                {
                    List<string> stringD = LCSGen.StringSplit(D[i].ToString()); //分解Itemset为Item数组.
                    int countD = stringD.Count;
                    string[] subD = new string[countD];
                    stringD.CopyTo(subD);

                    for (int j = 0; j < I.Count; j++)
                    {
                        List<string> stringI = LCSGen.StringSplit(I[j].ToString());
                        int countI = stringI.Count;
                        string[] subI = new string[countI];
                        stringI.CopyTo(subI);

                        bool subIInsubD = true;//序列subI是否包含于序列subD
                        int nn = 0;//记录subD搜索字符的位置，第一次从头开始
                        for (int m = 0; m < subI.Length; m++)
                        {
                            bool subImInsubD = false;
                            for (int n = nn; n < subD.Length; n++)
                                if (subI[m] == subD[n])
                                {
                                    subImInsubD = true;
                                    nn = n + 1;//匹配上该字符，将nn(下一次扫描起始点定于n的后续字符)
                                    continue;
                                }
                            if (subImInsubD == false) subIInsubD = false;
                        }
                        if (subIInsubD == true) Icount[j]++;
                    }
                }

                //从初始序列中将支持度大于给定值的项转到L中  
                for (int i = 0; i < Icount.Length; i++)
                {
                    if (Icount[i] >= sup * D.Count)
                    {
                        Ifrequent.Add(I[i]);
                        ItemSet iSet = new ItemSet();
                        iSet.Items = I[i].ToString();
                        iSet.Sup = Icount[i];
                        L.Add(iSet);
                    }
                }
                C = Ifrequent;
                        
            }
        }
        public static List<ItemSet> Apriori(ArrayList D, ArrayList I, float sup)
        {
            List<ItemSet> L = new List<ItemSet>();//所有频繁序列集  
            if (I.Count == 0) return L;
            else
            {
                int[] Icount = new int[I.Count];//初始序列集计数器,初始化为0  
                ArrayList Ifrequent = new ArrayList();//初始序列集中的频繁序列集  

                //遍历序列集，对候选序列进行计数  
                for (int i = 0; i < D.Count; i++)
                {
                    List<string> stringD=LCSGen.StringSplit(D[i].ToString());
                    int countD=stringD.Count;
                    string[] subD = new string[countD];
                    stringD.CopyTo(subD);

                    for (int j = 0; j < I.Count; j++)
                    {
                        List<string> stringI = LCSGen.StringSplit(I[j].ToString());
                        int countI = stringI.Count;
                        string[] subI =new string[countI];
                        stringI.CopyTo(subI);
                        
                        bool subIInsubD = true;//序列subI是否包含于序列subD
                        int nn = 0;//记录subD搜索字符的位置，第一次从头开始
                        for (int m = 0; m < subI.Length; m++)
                        {
                            bool subImInsubD = false;
                            for (int n = nn; n < subD.Length; n++)
                                if (subI[m] == subD[n])
                                {
                                    subImInsubD = true;
                                    nn = n + 1;//匹配上该字符，将nn(下一次扫描起始点定于n的后续字符)
                                    break;
                                }
                            if (subImInsubD == false) subIInsubD = false;
                        }
                        if (subIInsubD == true) Icount[j]++;
                    }
                }

                //从初始序列中将支持度大于给定值的项转到L中  
                for (int i = 0; i < Icount.Length; i++)
                {
                    if (Icount[i] >= sup * D.Count)
                    {
                        Ifrequent.Add(I[i]);
                        ItemSet iSet = new ItemSet();
                        iSet.Items = I[i].ToString();
                        iSet.Sup = Icount[i];
                        L.Add(iSet);
                    }
                }

                I.Clear();
                I = AprioriGen(Ifrequent,C);

                L.AddRange(Apriori(D, I, sup));
                return L;
            }
        }
        #endregion-----用Apriori算法进行迭代-----

        #region-------Apriori-gen生成候选项集---------
        /// <summary>  
        /// Apriori-gen生成候选序列集 
        /// </summary>  
        /// <param name="L">上一代频繁序列集</param>  
        /// <param name="C">频繁字符集</param> 
        /// <returns>这一代候选序列集</returns>  
        public static ArrayList AprioriGen(ArrayList L, ArrayList C)
        {
           ArrayList Lk = new ArrayList();
           for (int i = 0; i < L.Count; i++)
            {
                foreach (string c in C)
                    Lk.Add(L[i]+c);              
            }
            return Lk;
        }
        #endregion-------Apriori-gen生成候选项集--------- 
    }
}
