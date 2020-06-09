using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
    //正则表达提取算法接口
    interface IAlgorithm
    {
        /*执行算法
         * 输入参数I是
         */ 
       string execute(List<string> I);
    }
}
