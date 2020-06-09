using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
    class CommonSubstring
    {
        public MZQString s;
        public List<int> slots;//槽位置 0,1,..,sn，sn是s的长度。
        public List<int> covers;//覆盖的输入字符序列的位置。
        public CommonSubstring()
        {
            covers = new List<int>();
            slots = new List<int>();
        }
    }
}
