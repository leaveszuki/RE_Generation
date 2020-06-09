using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
     
    class MZQString:List<string>
    {
        public static List<MZQString> A = new List<MZQString>();    
        public int Length //字符序列长度
        {
            get { return this.Count; }
        }
        //将普通字符串转换化MZQSTring
        public MZQString(string arg)
        {            
            for (int i = 0; i < arg.Length; i++)
            {             
               char[] x = { arg[i] };

               string MZQchar = new string(x);          

               this.Add(MZQchar);                              
            }           
        }
        //将字符串数组转化为MZQString
        public MZQString(string[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
            {   
                string s = arg[i];

                this.Add(s);
            }
        }
        public MZQString()
        { 
        }
        //求子序列
        public MZQString Substring(int startIndex,int length)
        {
            MZQString result = new MZQString();
            int i=0;
            while (i < length)
            {
                result.Add(this[startIndex + i]);
                i++;
            }
            return result;
        }
        public static bool operator ==( MZQString s1, MZQString s2)
        {
            if ((System.Object)s1 == null && (System.Object)s2 == null)
            {
                return true;
            }
            if ((System.Object)s1 == null && (System.Object)s2 != null)
            {
                return false;
            }
            if ((System.Object)s1 != null && (System.Object)s2 == null)
            {
                return false;
            }
            
            return s1.Equals(s2);
        }
        public static bool operator !=( MZQString s1,MZQString s2)
        {
            if ((System.Object)s1 == null && (System.Object)s2 == null)
            {
                return false;
            }
            if ((System.Object)s1 == null && (System.Object)s2 != null)
            {
                return true;
            }
            if ((System.Object)s1 != null && (System.Object)s2 == null)
            {
                return true;
            }
            return !s1.Equals(s2);
        }
        //public bool Equals(MZQString p)
        //{
           
        //}      

        public static MZQString operator + (MZQString s1, MZQString s2)
        {           
            MZQString result = new MZQString();
            if ((System.Object)s1 != null)
            {
                for (int i = 0; i < s1.Length; i++)
                    result.Add(s1[i]);
            }
            if ((System.Object)s2 != null)
            { 
                for (int i = 0; i < s2.Length;i++ )
                    result.Add(s2[i]);

            }
                        return result;
        }

        public int IndexOf(MZQString s, int startIndex)
        {
            //先将MZQString转换为String，在调用String.IndexOf返回索引
            int i = 0;//指向this
            int j = 0;//指向参数s
            while ((i < this.Length)&&(j < s.Length))
            {
                if (this[i] == s[j])
                {
                    j++;
                }
                else
                {
                    j = 0;
                }
                i++;                
            }
            if (j == s.Length) return i - s.Length;
            else return -1;
        }

        public bool StartsWith(MZQString s)
        {
            if (this.Length > s.Length)
                return s.Equals(this.Substring(0, s.Length));
            else
                return false;
        }

        public bool EndsWith(MZQString s)
        {
            if (this.Length > s.Length)
                return s.Equals(this.Substring(this.Length - s.Length, s.Length));
            else
                return false;
        }
        /*
          * 功能：与s比较，返回公共后缀，若无公共后缀，则返回空。
          * 参数：s定义待比较的字符序列。   
          */
        public MZQString getCommonSuffix(MZQString s)
        {
            MZQString result=new MZQString();
            int i = Length - 1;
            int j = s.Length - 1;
            while ((i>=0)&&(j>=0)&&(this[i] == s[j])) 
            {
                result.Insert(0, this[i]);
                i--;
                j--;
            }
           return result;
        }
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < this.Length; i++)
                result = result + this[i];

           // return util.RegxHandler.simplify(result);

            return result;
        }    
        ////严格比较：判断每个元素是否相等，当两个MZQString展开后相等，但内部划分到元素的方式不同时，返回不等的结果。
        //public override bool Equals(object obj)
        //{          
        //    MZQString p=(MZQString)obj;
        //    //如果长度不等，return false
        //    if (this.Length != p.Length)
        //    {
        //        return false;
        //    }
        //    //比较MZQString中每个字符串是否相等
        //    bool flag = true;
        //    for (int i = 0; i < this.Length; i++)
        //    {
        //        if (this[i] != p[i])
        //        {
        //            flag = false;
        //            break;
        //        }
        //    }
        //    return flag;           
        //}
        //不严格比较：比较展开后的string型字符串，不管内部划分方式是否相同。
        public override bool Equals(object obj)
        {
            MZQString p = (MZQString)obj;

            string ps = p.ToString();
            string s = this.ToString();
                        
            return s.Equals(ps);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        //判断是否包含元字符，即长度大于1的辅助字符且以A开头，返回所有元字符的索引集合。
        public List<int> GetMetaChars()
        {
            List<int> result = new List<int>();
            for (int i = 0; i < Length; i++)
            {
                if (this[i].Length > 1 && this[i].Substring(0,1)=="A") result.Add(i);
            }
            return result;
        }
        //功能：将所有有连续重复次数大于等于阈值r的子序列替换成相应表示正则表达式的辅助字符，以描述序列的循环模式。
        //A数组存储所有辅助字符所代表的正则表达式。例如：辅助字符"A1"所代表的正则表达式存储在A[1]中。
        //问题：当引入辅助字符后，字符串中包含大量*操作符。*包含的内容允许出现0次，这样就与未出现*的字符串存在重复的可能。目前这种情况无法检测。20170915。这个问题存疑。如果加入该
        //机制可能会引入过度泛化。
        public MZQString DiscoverSeqPattern(int r)
        {
            bool existSeqPattern = false;//存在需要替换的循环模式。
            MZQString result = new MZQString();
            for (int i = 0; i < Length; i++) result.Add(this[i]);

            int ws = 1; //ws定义扫描窗口大小，即重复子序列长度，从1开始
            while (ws < result.Length)
            {
                int i = 0;//i定义扫描窗口位置，从0开始
                int x = 0;//x定义连续重复子序列的个数
                while (i + 2 * ws - 1 < result.Length)
                {
                   while ((i + (x + 2) * ws - 1 < result.Length) && (result.Substring(i + ws * x, ws) == result.Substring(i + ws * (x + 1), ws)))
                    {
                        x++;
                    }
                    if (x >= r - 1)
                    { 
                        //替换成正则表达式
                        MZQString ts = result.Substring(i,ws);                      
                        MZQString regs;
                        if (ts.Toplainstring().Length > 1) regs = new MZQString("(") + ts + new MZQString(")*");
                        else regs = ts + new MZQString("*");
                       
                        int index = A.Count;

                        //判断是否A集合中已存在regs。若已存在，则直接引用，否则在A末尾添加一个新元素。
                        //注意当A比较大时，此处将成为制约算法性能的瓶颈。
                        
                        for(int k=0;k<A.Count;k++)
                        {
                           if(A[k]==regs) index=k;
                        }                            
                            
                        string regm = "A" + index;
                        
                        if(index==A.Count) A.Add(regs);

                        result.RemoveRange(i,(x+1)*ws);

                        result.Insert(i,regm);
                        existSeqPattern = true;
                    }
                    x = 0;
                    i++;
                }
                ws++;
            }
            if (existSeqPattern) return result.DiscoverSeqPattern(r);
            else return result;
        }

       
        /*
     * 功能：如果在字符串si中，字符a1,a2,…,am在近距离内多次重复出现，则认为字符串si很可能源自正则表达式（a1|a2|…|am）*。
     * 参数：s定义输入字符序列，d为判断区域性的距离参数。   
     */
        public MZQString DiscoverOrPattern(int d)
        {
            List<MZQString> r = Partition(d);
            MZQString result = new MZQString();
            foreach (MZQString ss in r)
            {
                if (ss.Length > 1)
                {
                    string[] cc = ss.ToArray<string>();
                    IEnumerator<string> cs = cc.Distinct<string>().GetEnumerator();
                    MZQString temp = new MZQString("(");
                    while (cs.MoveNext())
                    {
                        string c = (string)cs.Current;

                        MZQString MZQc = new MZQString();
                        
                        MZQc.Add(c);

                        temp = temp + MZQc + new MZQString("|");
                    }
                    temp = temp.Substring(0, temp.Length - 1);
                    temp = temp + new MZQString(")*");

                  

                    //确定辅助字符
                    int j = A.Count;
                    int i = 0;
                    while (i < A.Count) 
                    {
                        if (A[i] == temp) {
                            j = i;
                            break;
                        }
                        i++;
                    }
                    if (j == A.Count) A.Add(temp);

                    result.Add("A" + j);
                }
                else
                {
                    result = result + ss;
                }
            }
            return result;
        }
        /*
功能：序列s分割成许多短小的子序列s1,s2,…,sn，使得对于si中任何字符a，在其他的子序列sj中都不会出现，即使出现相同字符，该相同字符在序列S中的位置离si中该字符的位置
      也大于参数d。
参数：s定义输入字符序列，d为判断区域性的距离参数。         
    */
        public List<MZQString> Partition(int d)
        {
            List<MZQString> shortstring = new List<MZQString>();

            int start, end;
            start = end = 0;

            while (end < this.Length)
            {
                MZQString ss = new MZQString();
                int l = 0;
                int k = 0;//子序列扩充的幅度。
                while ((end < this.Length) && (l <= d))
                {
                    end = end + k;//此处与论文原算法不同。子序列扩展的幅度不是l，而是d。l是右侧出现相同字符与子序列内字符距离，d是右侧出现相同字符与子序列尾部的距离。

                    ss = this.Substring(start, end - start + 1);
                    l = int.MaxValue;//ss中字符出现在ss右侧的位置距离ss中该字符位置的最小值
                    for (int i = 0; i < ss.Length; i++)
                    {

                        if ((end + 1 < this.Length) && (this.IndexOf(ss[i], end + 1) > 0))
                        {
                            l = this.IndexOf(ss[i], end + 1) - (i + start);//获取ss[i]的右侧再次出现的位置与ss[i]位置的距离。i+start是字符ss[i]在s中的位置。
                            if (l <= d)
                            {
                                k = this.IndexOf(ss[i], end + 1) - end;
                                break;//若距离小于d，则不再查找下一个字符，直接返回上层循环。
                            }
                        }
                    }
                }
                if (end < this.Length)//往右跳一大步，跳过刚识别的子序列。
                {
                    shortstring.Add(ss);
                    start = end + 1;
                    end = end + 1;

                }
            }
            return shortstring;
        }

       /*
        * 功能：生成前缀集合。
        * 参数：返回所有前缀集合。   
        */
        public List<MZQString> getPrefixes()
        {
            List<MZQString> result = new List<MZQString>();
            for (int i = 0; i < Length; i++) result.Add(Substring(0, i + 1));
            return result;
        }
       /*
        * 功能：生成后缀集合。
        * 参数：返回所有前缀集合。   
        */
        public List<MZQString> getsuffixes()
        {
            List<MZQString> result = new List<MZQString>();
            for (int i = 0; i < Length; i++) result.Add(Substring(Length - 1 - i, i + 1));
            return result;
        }
        /*
        * 功能：把所有辅助字符替换成基本字符。
        * 参数：返回保包含基本字符的MZQString字符串。   
        */    
        public MZQString Toplainstring()
        {
            MZQString result = new MZQString();
            bool existAuxiChar = false;
            for (int i = 0; i < this.Length; i++)
            {
                string c = this[i];

                //当字符c长度大于1，且以“A”开头时，为辅助字符

                if (c.Length > 1 && c.Substring(0,1)=="A")
                {
                    existAuxiChar = true;
                    int index =Int32.Parse(c.Substring(1, c.Length - 1));
                    if(i==0 && i+1==this.Length) result = result + dropEmbrace(A[index]); //表达式只包括一个辅助字符，去掉辅助字符内部的最外层括号。
                    else if (i>0 && this[i - 1] == "(" && i+1<this.Length && this[i + 1] == ")") result = result + dropEmbrace(A[index]);//辅助字符外侧直接包围括号，去掉辅助字符内部的最外层括号。
                    else result = result + A[index];
                }
                else 
                {
                    result.Add(c);
                }
            }
            if (existAuxiChar) return result.Toplainstring();
                return result;                 
        }
        //克隆一份MZQString。
        public MZQString clone()
        {
            MZQString result = new MZQString();
            for (int i = 0; i < this.Count; i++)
            {
                result.Add(this[i]);
            }
            return result;               
        }
        //去除外层括号
        public MZQString dropEmbrace(MZQString para)
        {

            MZQString result=para.clone();

            if (result.Count > 0 && result[0] == "(" && result[result.Length - 1]==")")
            {
                result.RemoveAt(0);
                result.RemoveAt(result.Length - 1);
            }

            return result;
        }

        public void convertChar()
        {
            //遇到正则表达式元字符，用转义字符替代，加上\ 
            for (int i = 0; i < this.Count; i++)
            {
                if ((this[i] == ".") || (this[i] == "\\") || (this[i] == "*"))

                    this[i] = "\\" + this[i];
            }
            return;
        }
    }//end of class
    
}
