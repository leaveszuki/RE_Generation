using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MZQStringRuleMining.util
{
    //提供一些正则表达式拼接、化简等运算操作。
    class RegxHandler
    {
        //看正则表达式片断是否适合作为连接操作的操作数使用。
        public static bool canUsedAsAndOperator(string reg)
        {
            List<string> words = divideTopLevelString(reg);
            //若words多于一个元素，且包括或操作符则不适合，否则适合。
            if (words.Count > 1 && words.Contains("|"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //将reg按顶层|连接符切分成多个子操作符并返回子操作符列表。
        public static List<string> divideOr(string reg)
        {
            List<string> awords = divideTopLevelString(reg);
            List<string> acomponents = new List<string>();
            int index;
            while ((index = awords.IndexOf("|")) > 0)
            {//存在|操作符 
                if (index == 1)
                {
                    //看是否带括号，若带括号则去掉括号
                    if (awords[0][0] == '(' && awords[0][awords[0].Length - 1] == ')') acomponents.Add(awords[0].Substring(1, awords[0].Length - 2));
                    else acomponents.Add(awords[0]);
                }
                else
                {
                    string str = "";
                    for (int i = 0; i < index; i++) str += awords[i];
                    acomponents.Add(str);
                }
                awords.RemoveRange(0, index + 1);
            }
            if (awords.Count == 1)
            {
                //看是否带括号，若带括号则去掉括号
                if (awords[0][0] == '(' && awords[0][awords[0].Length - 1] == ')') acomponents.Add(awords[0].Substring(1, awords[0].Length - 2));
                else acomponents.Add(awords[0]);
            }
            else
            {
                string str = "";
                for (int i = 0; i < awords.Count; i++) str += awords[i];
                acomponents.Add(str);
            }
            return acomponents;
        }
        //正则表达式rega与正则表达式regb做|连接。
        public static string Or(string rega, string regb)
        {
            if (rega == "") return regb;
            if (regb == "") return rega;
            //if (includes(rega, regb)) return rega;
            //if (includes(regb, rega)) return regb;
            string  result=rega;
            List<string> bc = divideOr(regb);

            foreach (string b in bc)
            {
                if (!includes(rega, b)) result += "|" + b; 
            }
            return result;
        }
        //判断正则表达式rega是否包含正则表达式regb。
        public static bool includes(string rega, string regb)
        {
            //如果最外层是括号，则去除最外围括号
            while (divideTopLevelString(rega).Count == 1 && rega[0] == '(' && rega[rega.Length - 1] == ')') rega = rega.Substring(1, rega.Length - 2);
            while (divideTopLevelString(regb).Count == 1 && regb[0] == '(' && regb[regb.Length - 1] == ')') regb = regb.Substring(1, regb.Length - 2);
            
            if (rega.Length == 1 && regb.Length == 1)
            {
                if (rega == regb) return true;
                else return false;
            }

            //如果rega或regb是单个字符加*结尾，则去掉*，再比较
            //暂不处理带表达式的闭包化简问题——祖祈注
            Regex starRegexsingle = new Regex(@"^.(\*)+$"); //原正则表达式为^.(\*)+$
            Regex starRegexcomplex = new Regex(@"^[(].+[)](\*)+$");
            bool astar = false;
            bool bstar = false;

            if (rega.Length == 2) astar = starRegexsingle.IsMatch(rega);
            else astar = starRegexcomplex.IsMatch(rega);
            if (regb.Length == 2) bstar = starRegexsingle.IsMatch(regb);
            else bstar = starRegexcomplex.IsMatch(regb);

            if (astar)
            {
                string a_basestring = rega.Replace("*", "");
                string b_basestring = regb.Replace("*", "");

                if (a_basestring == b_basestring) return true;
                else return false;
            }
            if (!astar && bstar) {
                return false;
            }

            List<string> ac = divideOr(rega);
            List<string> bc = divideOr(regb);
            if (ac.Count == 1 && bc.Count == 1)
            {//没有或连接
                List<string> aw = divideTopLevelString(ac[0]);
                List<string> bw = divideTopLevelString(bc[0]);

                int i = 0;
                int j = 0;

                int acount=aw.Count;
                int bcount=bw.Count;

                while (j < bcount && i < acount)
                {
                    if (includes(aw[i], bw[j])) {
                        i++; j++; 
                    }
                    else
                    {
                        i++; j = 0;
                    }
                }
                if (j == bcount) return true;
                else return false;
            }
            //若regb的每部分都被rega包含，则regb被rega包含。
            foreach (string b in bc)
            {
                bool r = false;
                foreach (string a in ac)
                {
                    if (includes(a, b)) {
                        r = true;
                        break;
                    }
                }
                if (!r) return false;              
            }
            return true;
        }
        /*
         * 对reg进行预处理，识别顶层词汇，主要处理顶层括号和*运算符，将顶层括号包含的字符看作一个词汇。
         *未被顶层括号包含的每个字符都看作一个独立词汇，
         *如果在顶层出现*运算符，就将*运算符归入前面紧邻的词汇。若在括号内出现*运算符，则先不作处理。
         *将reg分割成words列表。
         */
        public static List<string> divideTopLevelString(string reg)
        {
           
            List<string> words = new List<string>();//reg的顶层词汇表
            string temps = "";
            int KuaoHaoLevel = 0;//括号层级，处理多级括号的情况。0级就是不在括号内。
            for (int iq = 0; iq < reg.Length; iq++)
            {
                if (KuaoHaoLevel == 0)//不在括号内
                {
                    if (reg[iq] != '(')
                    {
                        if (reg[iq] != '*')
                        {
                            //每读入一个字符，作为一个独立词汇添加到词表表words中。
                            temps = "";//清空临时词汇 temps
                            temps += reg[iq];
                            words.Add(temps);
                        }
                        else
                        {
                            words[words.Count - 1] += reg[iq];//若为*号，则归入words最后一个词汇的尾部。                            
                        }
                    }
                    else
                    { //遇到第一个左括号                                              
                        temps = "(";//清空临时词汇 temps，不能忽略左括号。
                        KuaoHaoLevel++;//括号层级加1;
                    }
                }
                else if (KuaoHaoLevel > 0)//在括号内，并且不是右括号，则拼接到temps中。
                {
                    if (reg[iq] == '(') KuaoHaoLevel++; //在括号中又遇到左括号,括号层级加1;
                    if (reg[iq] == ')') KuaoHaoLevel--;//遇到右括号，括号层级减1；      
                    temps += reg[iq];
                    if (KuaoHaoLevel == 0)//说明这是顶层右括号，完成一个词汇识别。不能忽略该右括号。
                    {
                        words.Add(temps);
                        temps = "";
                        continue;
                    }
                }
            }
            return words;
        }//ends of divideTopLevelString
        /*
         * 化简正则表达
         */
        public static string simplify(string spara)
        { 
           //若顶层Word数量为1，且该Word的最外层是括号，则扒掉括号。
          
           //if(divideTopLevelString(spara).Count==1 && spara[0]=='(' && spara[spara.Length-1]==')') {
           //    spara=spara.Remove(0, 1);
           //    spara=spara.Remove(spara.Length-1,1);
           //}

           return spara;
        }

    }
}
