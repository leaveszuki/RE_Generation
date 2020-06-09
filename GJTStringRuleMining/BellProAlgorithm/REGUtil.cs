using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MZQStringRuleMining.BellProAlgorithm
{
    //处理正则表达式的常用函数
    class REGUtil
    {

        public struct TRIPLET
        {
            public HashSet<MZQString> V;
            public HashSet<MZQString> Q;
            public HashSet<MZQString> R;
        }         

        /*
       * 功能：在正则表达式集合S中，计算正则表达D的分数，表征D作为备选模式的价值。S中每个正则表达式与S中许多
       * 其他正则表达式拥有共同的前缀或后缀。因此，可以认为如果S中正则表达式拥有越多的共同前缀或后缀，或共同
       * 前缀或后缀的长度越长，生成的分解形式质量就越高。
       * 参数：D定义需要计算分值的目标正则表达式，S是正则表达式D所在的正则表达式集合。   
       */
        public static int score(MZQString D, List<MZQString> S)
        {
            int s = 0;          
            List<MZQString> prefD = D.getPrefixes();
            List<MZQString> sufD = D.getsuffixes();

            for (int i = 0; i < prefD.Count; i++)
            {
                MZQString p = prefD[i];
                int sum=0;
                for (int j = 0; j < S.Count; j++)
                {
                    if (S[j].StartsWith(p)) sum++;
                }
                if (p.Toplainstring().Length * sum > s) s = p.Toplainstring().Length * sum;
            }

            for (int i = 0; i < sufD.Count; i++)
            {
                MZQString p = sufD[i];
                int sum = 0;
                for (int j = 0; j < S.Count; j++)
                {
                    if (S[j].EndsWith(p)) sum++;
                }
                if (p.Toplainstring().Length * sum > s) s = p.Toplainstring().Length * sum;
            }
                return s;
        }
     /*
      * 功能：计算SG中正则表达式覆盖的输入字符序列的集合。
      * 参数：   
      */
        public static void initCover(List<MZQString> SG, out List<HashSet<int>> Covers, List<string> I)
        {
            Covers = new List<HashSet<int>>();
            try
            {
                for (int i = 0; i < SG.Count; i++)
                {
                    MZQString R = SG[i];
                    Covers.Add(new HashSet<int>());
                    Regex reg = new Regex(R.Toplainstring().ToString());
                    for (int j = 0; j < I.Count; j++)
                    {
                        if ((i == 314) && (j == 49)) {
                            Console.WriteLine("here");
                        }
                        if (reg.IsMatch(@I[j]))
                        {                          
                            Covers[i].Add(j);
                        }
                        else
                        {
                        }
                        Console.WriteLine("i:" + i + "/" + reg.ToString() + "; j:" + j + "/" + I[j]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
      
        /*
      * 功能：查找所有除数。也就是所有在S中拥有公共后缀的前缀的集合。如果在S中有两个或两个以上的序列拥有共同后缀s，那么
      * 截去后缀s剩下的子序列就是除数。将所有符合该条件的除数集合（前缀）返回。 
      * 参数：S是要处理的字符序列集合。
      */
        public static HashSet<HashSet<MZQString>> FindAllDivisors(List<MZQString> S)
        {
            HashSet<HashSet<MZQString>> DivisorSet = new HashSet<HashSet<MZQString>>();//构建一个空的除数集合。

            for(int i=0;i<S.Count;i++)
                for (int j = i + 1; j < S.Count; j++)
                {
                    HashSet<MZQString> Divisor = new HashSet<MZQString>();
                   MZQString suffix = S[i].getCommonSuffix(S[j]);
              //      if (suffix.Length > 0 && !suffixes.Contains(suffix)) 
                    if (suffix.Length > 0)
                    {
                      //  suffixes.Add(suffix);
                        foreach(MZQString s in S)//将S中所有以suffix作为后缀的字符序列找到，并且取出其剩余前缀
                        {
                            if (s.EndsWith(suffix)) Divisor.Add(s.Substring(0, s.Length - suffix.Length));
                        }
                        //Divisor.Add(S[i].Substring(0,S[i].Length-suffix.Length));
                        //Divisor.Add(S[j].Substring(0, S[j].Length - suffix.Length));
                    }
                    if (Divisor.Count == 0) continue;
                    
                    //判断新产生的Divisor在DivisorSet中是否存在。或已存在则不用添加。若不存在就添加。
                    bool isExist = false;
                    foreach (HashSet<MZQString> s in DivisorSet)
                    {
                        if (s.Count == Divisor.Count)
                        {
                            int c = 0;//Divisor匹配字符串数量
                            foreach (MZQString d in Divisor)
                            {
                                if (!s.Contains(d)) break;
                                c++;
                            }
                            if (c == Divisor.Count)
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }
                    if(!isExist) DivisorSet.Add(Divisor);
                    
                }
            return DivisorSet;
        }
        /*功能：除法运算。S除V，Q是商数，V是余数。
         * 参数：S是被除数，V是除数。
         */
        public static void Divide(HashSet<MZQString> S, HashSet<MZQString> V, out HashSet<MZQString> Q,out HashSet<MZQString> R)
        {   
            List<HashSet<MZQString>> qps=new List<HashSet<MZQString>>();
            foreach (MZQString p in V)
            {
                HashSet<MZQString> qp = new HashSet<MZQString>();
                foreach (MZQString s in S)
                {
                    if (s.StartsWith(p)) qp.Add(s.Substring(p.Length,s.Length-p.Length));
                }
                qps.Add(qp);
            }
            Q = new HashSet<MZQString>();
            R = new HashSet<MZQString>();

            if(qps.Count>0 ) Q=qps[0];
            for (int i = 1; i < qps.Count; i++) Q.IntersectWith(qps[i]);

            HashSet<MZQString> TEMP = new HashSet<MZQString>();
            
            foreach (MZQString p in V)
                foreach (MZQString q in Q)
                    TEMP.Add(p + q);
            R = S;
            R.ExceptWith(TEMP);
        }

        /*功能：通过查找频繁子序列的方式将字符序列集合S进行分解（化简）
         * 参数：S是待处理的字符序列集合
         * */
        public static MZQString Factor_Based_On_Apriori(List<MZQString> S)
        {
            MZQString result = new MZQString("");
          //1.查找备选频繁子序列

          //2.按频繁子序列的频繁度与支持度的乘积，对子序列进行排序

          //3.按顺序逐步选取前n个频繁子序,使得所代表的正则表达式的或联接刚好覆盖输入字符序列集合S中所有字符序列

          //4.返回将正则表达式用“|”逻辑联接符联接，最终构造化简结果。

            return result;
        
        }
      
        /*
         * 功能：计算两个正则表达式匹配的字符序列集合的交集比例
         */
        public static double Overlap(MZQString D, MZQString D1, List<MZQString> SG, List<HashSet<int>> Covers)
        {

            try
            {
                int indexD = SG.IndexOf(D);
                int indexD1 = SG.IndexOf(D1);
                if (indexD < 0 | indexD1 < 0) return -1;
                HashSet<int> hdintersect = new HashSet<int>(Covers[indexD].ToArray<int>());
                hdintersect.IntersectWith(Covers[indexD1]);
                HashSet<int> hdunion = new HashSet<int>(Covers[indexD].ToArray<int>());
                hdunion.UnionWith(Covers[indexD1]);
                if (hdunion.Count == 0) return 0;
                return Math.Abs(hdintersect.Count / hdunion.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(D.Toplainstring().ToString());
                Console.WriteLine(D1.Toplainstring().ToString());
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        /*功能：计算编码字符串。
         * 参数：reg是正则表达式，s是待编码的字符序列。
         * 校审时间：2017-06-07(原规则B横向分析时字符间编码有空格，该次校对将空格去掉)
         * 二次校审时间：2017-01-01(原规则D缺少标明位数的多个1)
         * */
        public static MZQString SEQ(MZQString reg, MZQString s)
        {

                string plainreg = reg.Toplainstring().ToString();

                string pattern;
                //为匹配整个样例序列，给正则表达式两端增加^和$。
                if (util.RegxHandler.canUsedAsAndOperator(plainreg)) pattern = "^" + plainreg + "$";
                else pattern = "^(" + plainreg + ")$";

                Regex Rreg = new Regex(pattern);

                if (!Rreg.IsMatch(s.ToString())) return null;

                Match tempm = Rreg.Match(s.ToString());

                //规则A。如果reg不包含元字符，且reg与s相等，则返回空字符串。
                if ((reg.GetMetaChars().Count == 0) && !reg.Contains("|") && !reg.Contains("*")) return new MZQString("");

                /*
                 * (2)对reg进行预处理，识别顶层词汇，主要处理顶层括号和*运算符，将顶层括号包含的字符看作一个词汇。
                *未被顶层括号包含的每个字符都看作一个独立词汇，
                 *如果在顶层出现*运算符，就将*运算符归入前面紧邻的词汇。若在括号内出现*运算符，则先不作处理。
                 *将reg分割成words列表。
                 */
                List<MZQString> words = new List<MZQString>();//reg的顶层词汇表
                MZQString temps = new MZQString();
                int KuaoHaoLevel = 0;//括号层级，处理多级括号的情况。0级就是不在括号内。
                for (int i = 0; i < reg.Length; i++)
                {
                    if (KuaoHaoLevel == 0)//不在括号内
                    {
                        if (reg[i] != @"(")
                        {
                            if (reg[i] != @"*")
                            {
                                //每读入一个字符，作为一个独立词汇添加到词表表words中。
                                temps = new MZQString();//清空临时词汇 temps
                                temps.Add(reg[i]);
                                words.Add(temps);
                            }
                            else
                            {
                              //  words[words.Count - 1].Insert(0, @"("); 原来算法忽略到最外层括号，现算法未忽略，所以这里也不需要补充括号了。
                              //  words[words.Count - 1].Add(@")");
                                words[words.Count - 1].Add(reg[i]);//若为*号，则归入words最后一个词汇的尾部。
                            }
                        }
                        else
                        { //遇到第一个左括号                                              
                            temps = new MZQString(@"(");//清空临时词汇 temps，不能忽略左括号。
                            KuaoHaoLevel++;//括号层级加1;
                        }
                    }
                    else if (KuaoHaoLevel > 0)//在括号内，并且不是右括号，则拼接到temps中。
                    {
                        if (reg[i] == @"(") KuaoHaoLevel++; //在括号中又遇到左括号,括号层级加1;
                        if (reg[i] == @")") KuaoHaoLevel--;//遇到右括号，括号层级减1；      
                        temps.Add(reg[i]);
                        if (KuaoHaoLevel == 0)//说明这是顶层右括号，完成一个词汇识别。不能忽略该右括号。
                        {                           
                            words.Add(temps);
                            temps = new MZQString();
                            continue;
                        }
                        
                    }
                }
            
               //若words只有一个元素，且该元素最外面被（）包围，则扒掉（）。
                if (words.Count == 1 && words[0][0] == "(" && words[0][words[0].Length - 1] == ")")
                {
                    words[0] = words[0].Substring(1, words[0].Length - 2);
                    return SEQ(words[0],s);
                }

                //判断项层词汇属于哪种规则。
                //判断规则A 无辅助字符，无 | 无* 

                //如果words只有一个元素，且最后一个字符是*，那就属于规则D
                //if ((reg.Length == 2 && reg[reg.Length - 1] == "*") || (reg[0] == "(" && reg[reg.Length - 2] == ")"
                //    && reg[reg.Length - 1] == "*"))
                 if(words.Count == 1&& words[0][words[0].Length - 1] == "*")
                {
                    MZQString reg1 = words[0].Substring(0, words[0].Length - 1);//去掉*号
                    while ((reg1[0] == "(") && (reg1[reg1.Length - 1] == ")"))//若reg1头尾有()包围，则去掉。
                    {
                        reg1 = reg1.Substring(1, reg1.Length - 2);
                    }


                    Regex rx = new Regex(reg1.Toplainstring().ToString()); //应该去年元符号，否则无法匹配。

                    //int regcount = 0;

                    MatchCollection ms = rx.Matches(s.ToString());

                    MZQString resultD = new MZQString(Convert2(ms.Count));

                    for (int i = 0; i < ms.Count; i++) resultD = resultD + SEQ(reg1, new MZQString(ms[i].ToString()));//中间不加空格
                    //for (int i = 0; i < ms.Count; i++) resultD = resultD + new MZQString(" ") + SEQ(reg1, new MZQString(ms[i].ToString()));

                    return resultD;

                }
                //最外层无*号，包含顶层|运算符，属于规则C
                else if (words.Contains(new MZQString("|")))
                {
                    MZQString resultC = new MZQString();
                    
                    while ((words[0] == new MZQString("(")) && (words[words.Count - 1] ==new MZQString(")")))//若words头尾有()包围，则去掉。
                    {
                        words.RemoveAt(0);
                        words.RemoveAt(words.Count);
                    }

                    int i = 0;
                    int count = 0;//相当于算法中的m
                    bool found = false;
                    while (words.IndexOf(new MZQString("|"), 0) > 0)//此处应该是word的|
                    {
                        MZQString rr = new MZQString("");

                        //拼接|之前的字符序列，去除|之前的字符序列.  
                        int index = words.IndexOf(new MZQString("|"), 0);
                        for (int kj = 0; kj < index; kj++)
                        {
                            rr=rr+words[0];
                            words.RemoveAt(0);
                        }
                        words.RemoveAt(0);//去除|本身。                
                       
                        Regex rreg= new Regex(rr.Toplainstring().ToString());
                        
                        if (rreg.IsMatch(s.ToString()))
                        {
                            resultC = SEQ(rr, s);
                            found = true;
                            i = count;
                        }
                        count++;
                    }

                    count++;//把|符号后面的部分也加入。

                    if (!found)
                    { //若未找到，判断最后一个reg1.

                        MZQString rr = new MZQString("");
                                                
                        for (int kj = 0; kj < rr.Count; kj++)
                        {
                            rr = rr + words[kj];                         
                        }
                        Regex rreg = new Regex(rr.Toplainstring().ToString());

                        if (rreg.IsMatch(s.ToString()))
                        {
                            resultC = SEQ(rr, s);
                            i = count - 1;
                            found = true;
                        }
                    }

                    double bits = Math.Log(count, 2);
                    int intb = 0;
                    if ((int)bits == bits) intb = (int)bits + 1;
                    else intb = (int)bits + 1;

                    string i2 = Convert.ToString(i, 2).PadLeft(intb, '0');

                   // if (found) resultC = new MZQString(i.ToString()) + new MZQString(" ") + resultC;
                    if (found) resultC = new MZQString(i2) + resultC; //中间不要加空格
                    else Console.WriteLine("异常：在|中，未找到匹配项。正则表达式：" + reg + "字符序列：" + s);
                    return resultC;
                }
                //规则B：橫向分解
                else
                {
                    MZQString resultB = new MZQString();
                    int i = 0;
                    int j = 0;
                    while ((i < words.Count) && (j < s.Length))
                    {
                        MZQString tw = words[i];//临时词汇变量，存储当前需处理的词汇。
                        string c = tw.Toplainstring().ToString();
                        if (c.Equals("\\")) 
                        {
                            MZQString tw2 = words[i + 1];
                            string c2 = tw2.Toplainstring().ToString();
                            if (c2.Equals("."))//应该将\.组合到一起，当成转义字符看待
                            {
                                c += c2;
                                i++;
                            }
                            else//出现了单独的\，则正则表达式出错 
                            {
                                throw new Exception("正则表达式语法错误：正则表达式中不应该出现单独的\\符号");
                            }
                        }
                        //若末字符是* ，则转化成+，否则C#的Match()得不到匹配结果.
                        if (c[c.Length - 1] == '*')
                        {
                            c = c.Substring(0, c.Length - 1);
                            c = c + "+";
                        }

                        Regex rr = new Regex(c);

                        Match m = rr.Match(s.ToString());
                        MZQString reg1;

                        if ((tw.Length == 1) && (tw[0].Length > 1) && (tw[0].Substring(0,1)=="A"))
                        {//该词汇只有一个辅助字符，根据A列表对辅助字符进行解析和替换
                           // int index = Int32.Parse(reg[i].Substring(1, reg[i].Length - 1));//不理解当时为什么这么写
                            int index = Int32.Parse(tw[0].Substring(1,tw[0].Length-1));
                            reg1 = MZQString.A[index];
                        }
                        else reg1 = tw;

                        resultB = resultB + SEQ(reg1, new MZQString(m.ToString()));
                        //resultB = resultB + new MZQString(" ") + SEQ(reg1, new MZQString(m.ToString()));
                        i++;
                        j = j + m.Length;
                    }
                    return resultB;
                }          
        }
     
    //转换成算法所需要的二进制输出。    
     public static string Convert2(int arg)
     {
      string result="";
      double bits=Math.Log(arg, 2);
      int intb = 0;
      if((int)bits==bits) intb=(int)bits+1;
      else intb=(int)bits+1;
      
      for (int i = 0; i < intb; i++)
      result = result + "1";

      result = result + "0";

      result = result + Convert.ToString(arg, 2);
     
      return result;              
    }
     /*功能：将字符序列集合S进行因式分解（化简）
     * 参数：S是待处理的字符序列集合
     * */
     public static MZQString Factor(List<MZQString> S)
     {
         MZQString result = new MZQString();

         //获取所有除数
         HashSet<HashSet<MZQString>> DivisorSet = FindAllDivisors(S);

         //若没有除数，则直接返回输入字符序列的或连接形式
         if (DivisorSet.Count == 0)
         {
             foreach (MZQString s in S)
                 result = result + s + new MZQString("|");
             result = result.Substring(0, result.Length - 1);
             return result;
         }
         //定义存储除数、商、余数三元组集合
         List<TRIPLET> DivisorList = new List<TRIPLET>();

         foreach (HashSet<MZQString> v in DivisorSet)
         {
             HashSet<MZQString> Q;
             HashSet<MZQString> R;
             HashSet<MZQString> HS = new HashSet<MZQString>(S);
             HashSet<MZQString> HV = v;
             Divide(HS, HV, out Q, out R);
             TRIPLET t = new TRIPLET();
             t.V = v;
             t.Q = Q;
             t.R = R;
             DivisorList.Add(t);
         }
         int tripletLength = int.MaxValue;
         int index = -1;
         for (int i = 0; i < DivisorList.Count; i++)
         {
             MZQString gs = new MZQString();

             foreach (MZQString v in DivisorList[i].V) gs = gs + v;
             foreach (MZQString q in DivisorList[i].Q) gs = gs + q;
             foreach (MZQString r in DivisorList[i].R) gs = gs + r;

             if (gs.Length < tripletLength)
             {
                 tripletLength = gs.Length;
                 index = i;
             }
         }
         TRIPLET minTriplet = DivisorList[index];
         if (minTriplet.R.Count > 0)//如果有余数加|余数
             return new MZQString("(") + Factor(minTriplet.V.ToList<MZQString>()) + new MZQString(")") +
                 new MZQString("(") + Factor(minTriplet.Q.ToList<MZQString>()) + new MZQString(")") +
                  new MZQString("|") + Factor(minTriplet.R.ToList<MZQString>());
         else//如果没有余数就不用加|符号了。
             return new MZQString("(") + Factor(minTriplet.V.ToList<MZQString>()) + new MZQString(")") +
             new MZQString("(") + Factor(minTriplet.Q.ToList<MZQString>()) + new MZQString(")");

     }
    //根据输入字符序列I，统计字符集并存入zifuji集合
     public static void initZifuji(List<string> I, out HashSet<char> zifuji)
     {
         zifuji = new HashSet<char>();
         foreach (string s in I)
         {
             zifuji.UnionWith(s.ToArray<char>());
         }
     }
     //计算正则表达式的二进制表示长度
     public static int computeCodingLengthofREG(string s, int countofzf)
     {         
         int countofmetazf = 6;//论文中采用6个元字符，|，*，+，？，（，）。这里虽然暂时没用到？，但仍与论文保持一致。
         if (s == null) return 0;
         else
         {
             double bits = Math.Log(countofzf + countofmetazf, 2);
             int intb = 0;
             if ((int)bits == bits) intb = (int)bits + 1;
             else intb = (int)bits + 1;
             return s.Length * intb;
         }
     }
    /*
     * X 正则表达式集合
     * S 字符序列集合
     * CL 正则表达式与字符序列间匹配编码长度，若不匹配则为int.MaxValue
     */
     public static void computeCodingLengthsofString(List<MZQString> X, List<string> S, out int[,] CL)
     {         
         MZQString[,] codes;//正则表达式对字符序列的编码

         int CountofString = S.Count;//输入字符串数量

         int CountofReg = X.Count;//备选正则表达式数量

         codes = new MZQString[CountofReg, CountofString];

         for (int i = 0; i < CountofReg; i++)
         {            
             for (int j = 0; j < CountofString; j++)
                 codes[i, j] = REGUtil.SEQ(X[i], new MZQString(S[j]));
         }
         CL=new int[CountofReg, CountofString]; 
         
         for (int i = 0; i < CountofReg; i++)
             for (int j = 0; j < CountofString; j++)
             {              
                 if (codes[i, j] == null) CL[i, j] = int.MaxValue;
                 else if (codes[i, j].ToString() == "") CL[i, j] = 0;
                 else CL[i, j] = codes[i, j].ToString().Length;
             }
     }  
    
    }//end of class StringUtil    
}
