using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.BellProAlgorithm
{
    class Solution
    {
        public struct mapping {
            public int regIndex;//正则表达式在SG中的索引
            public int sIndex;//字符序列在X中索引
            public int codeLength;//编码长度
        }
        public List<int> regsIndexes;
        public List<mapping> mps;
        public int cost;       
    }
}
