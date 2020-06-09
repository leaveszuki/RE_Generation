using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.util
{
    class DataIO
    {
        private static string exportPath = "E:\\temp.csv"; //结果输出文件路径（结果存储在单个文件）

        
        //reCostList 正则表达式成本列表
        //decodeCostMatrix 编码成本列表
        static public void export_csv(int[] reCostList, int[,] decodeCostMatrix)
        {
            FileInfo fi = new FileInfo(exportPath);

            if (fi.Exists)
            {
                fi.Delete();
            }
            //  fi.Create();

            FileStream fs = new FileStream(exportPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            //写出正则表达式成本列表
            for (int i = 0; i < reCostList.Length; i++)
            {
                data = reCostList[i] + "";
                for (int j = 0; j < decodeCostMatrix.GetLength(1); j++)
                {
                    if (j < decodeCostMatrix.GetLength(1) - 1)
                    {
                        data += ";";
                    }                    
                }
                sw.WriteLine(data);
            }
            //写出各行数据
            for (int i = 0; i < decodeCostMatrix.GetLength(0); i++)
            {
                data = "";
                for (int j = 0; j < decodeCostMatrix.GetLength(1); j++)
                {
                    data += decodeCostMatrix[i, j];
                    if (j < decodeCostMatrix.GetLength(1) - 1)
                    {
                        data += ";";
                    }
                }
                sw.WriteLine(data);
            }               
            //关闭文件
            sw.Close();
            fs.Close();
        }


        //结果存储在两个文件
        private static string exportPathr = "E:\\r.csv"; //正则表达式自身编码成本文件路径（结果存储在两个文件）
        private static string exportPaths = "E:\\s.csv"; //正则表达式对样本序列的编码成本文件路径（结果存储在两个文件）
        //reCostList 正则表达式成本列表
        //decodeCostMatrix 编码成本列表
        static public void export_csv2(int[] reCostList, int[,] decodeCostMatrix)
        {
            FileInfo fir = new FileInfo(exportPathr);
            FileInfo fis = new FileInfo(exportPaths);

            //写正则表达式自身编码成本文件
            if (fir.Exists)
            {
                fir.Delete();
            }
            //  fir.Create();

            FileStream fsr = new FileStream(exportPathr, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter swr = new StreamWriter(fsr, System.Text.Encoding.Default);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string datar = "";

            //写出正则表达式成本列表
            for (int i = 0; i < reCostList.Length; i++)
            {
                datar = reCostList[i] + "";
                for (int j = 0; j < decodeCostMatrix.GetLength(1); j++)
                {
                    if (j < decodeCostMatrix.GetLength(1) - 1)
                    {
                        datar += ";";
                    }
                }
                swr.WriteLine(datar);
            }
            //关闭文件
            swr.Close();
            fsr.Close();


            //写出各行数据
            FileStream fss = new FileStream(exportPaths, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sws = new StreamWriter(fss, System.Text.Encoding.Default);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string datas = "";
            for (int i = 0; i < decodeCostMatrix.GetLength(0); i++)
            {
                datas = "";
                for (int j = 0; j < decodeCostMatrix.GetLength(1); j++)
                {
                    datas += decodeCostMatrix[i, j];
                    if (j < decodeCostMatrix.GetLength(1) - 1)
                    {
                        datas += ";";
                    }
                }
                sws.WriteLine(datas);
            }
            //关闭文件
            sws.Close();
            fss.Close();
        }
    }
}
