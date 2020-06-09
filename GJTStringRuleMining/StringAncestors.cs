using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
/***
 * 将ancestor 改成HashSet，便于集合与运算。
 * 
 * 该类将包含该字符串的原始字符串的编号记录在ancestor中。
 * 
 * 当字符序列重复较多时，即使字符序列不少的情况也，也会出一个子序列用这种数据结构会产生大量空余，空间复杂度非常大。
 * */
    class StringAncestors
    {
        public string s;//字符序列
        public HashSet<int> ancestors = new HashSet<int>();
        public StringAncestors()
        {
            s = "";
        }
        public StringAncestors(string args)
        {
            s = args;
        }
    }

}
