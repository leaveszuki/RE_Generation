using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
    class LCSGen
    {
        public static string StringConverge(List<string> s)
        {
            string R = "";
            for (int i = 0; i < s.Count;i++ )
            {
                R=R+s[i];
            }
            return R;
        }
        //将字符串arg拆分成包含多个字符串的字符串列表。每个字符串都包含一个字符
        public static List<string> StringSplit(string arg)
        {
            List<string> R = new List<string>();
            for (int i=0; i < arg.Length;i++ )
            {
                char[] x = { arg[i] };
                R.Add(new string(x));
            }
            return R;
        }
        // 采用Needleman-Wunsch算法计算字符串A和B的最长公共子序列
        public static List<string> NeedlemanWunsch(List<string> A, List<string> B)
        {
            int M = A.Count;
            int N = B.Count;
            List<string>[,] T = new List<string>[M + 1, N + 1];
            for (int i = 0; i <= M; ++i)
            {
                T[i, 0] = new List<string>();
            }
            for (int j = 1; j <= N; ++j)
            {
                T[0, j] = new List<string>();
            }
            for (int k = 1; k <= Math.Max(M, N); ++k)
            {
                if (k <= M)
                {
                    // 计算 T[k,*]
                    for (int n = Math.Min(k, N); n <= N; ++n)
                    {
                        if (T[k, n] == null)
                            T[k, n] = LCS(A[k - 1], B[n - 1], T[k, n - 1], T[k - 1, n], T[k - 1, n - 1]);
                    }
                }
                if (k <= N)
                {
                    // 计算 T[*,k]
                    for (int m = Math.Min(k, M); m <= M; ++m)
                    {
                        if (T[m, k] == null)
                            T[m, k] = LCS(A[m - 1], B[k - 1], T[m, k - 1], T[m - 1, k], T[m - 1, k - 1]);
                    }
                }
            }
            System.Diagnostics.Debug.Assert(T[M, N].Count <= Math.Max(A.Count, B.Count));
            return T[M, N];
        }
        // 计算单步LCS (如何处理长度相等的情况？)
        public static List<string> LCS(string a, string b, List<string> s1, List<string> s2, List<string> s3)
        {
            List<string> lcs = null;

            if (s1.Count > s2.Count)
                lcs = s1;
            else
                lcs = s2;

            if (a != b)
            {
                if (s3.Count > lcs.Count)
                    lcs = new List<string>(s3); // 拷贝而非引用
                else
                    lcs = new List<string>(lcs); // 拷贝而非引用
            }
            else
            {
                if (s3.Count + 1 > lcs.Count)
                {
                    lcs = new List<string>(s3); // 拷贝而非引用
                    lcs.Insert(lcs.Count, a);
                }
                else
                    lcs = new List<string>(lcs); // 拷贝而非引用
            }

            return lcs;
        }
    }
}
