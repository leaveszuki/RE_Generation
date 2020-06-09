using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//实验所用到的外部文件处理函数库
namespace MZQStringRuleMining
{
    class ExFileHandler
    {
        public static List<string> readFromFile(string path)
        {
            List<string> result = new List<string>();
            //逐行读取path文件，并转换成string数据，添加到result中。
            StreamReader sr = new StreamReader(path);
            while(!sr.EndOfStream)
            {
                result.Add(sr.ReadLine());                
            }

            return result;
        }
    }
}
