using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class ForthMethod
    {

        //Floyd算法，寻找最小回路集（A到B的最小路径加上B到A的最小路径），仅适用于十个状态以下的自动机
        public static List<List<string>> FloydCycle(StateMachine m)
        {
            List<List<string>> cycleset = new List<List<string>>();
            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            string[,] Ri = new string[count_States, count_States];
            //初始化数值
            for (int x = 0; x < count_States; x++)
            {
                for (int y = 0; y < count_States; y++)
                    Ri[x, y] = "##########";
            }
            foreach (State state in m.stateList)
            {
                foreach (Transition t in state.transitions)
                {
                    int i = Int32.Parse(state.identifier.Substring(1));
                    int j = Int32.Parse(t.target.identifier.Substring(1));
                    Ri[i, j] = i.ToString();
                }
            }
            //Floyd算法主体
            for (int z = 0; z < count_States; z++)
            {
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        if (Ri[x, y].Length > Ri[x, z].Length + Ri[z, y].Length)
                            Ri[x, y] = Ri[x, z] + Ri[z, y];
            }
            //将转移矩阵转化为二维列表
            for (int x = 0; x < count_States; x++)
            {
                for (int y = x; y < count_States; y++)
                {
                    if (Ri[x, y].Length < 10 && Ri[y, x].Length < 10)
                    {
                        List<string> cycle = new List<string>();
                        if (x == y && Ri[x, y].Length == 1)
                        {
                            //foreach (char c in Ri[x, y]) cycle.Add("M" + c);
                            //cycleset.Add(cycle);
                        }

                        else if (x != y)
                        {
                            bool flag = true;
                            foreach (char c1 in Ri[x, y]) foreach (char c2 in Ri[y, x]) if (c1 == c2) flag = false;
                            if (!flag) continue;
                            foreach (char c in Ri[x, y]) cycle.Add("M" + c);
                            foreach (char c in Ri[y, x]) cycle.Add("M" + c);
                            cycleset.Add(cycle);
                        }
                    }
                }
            }

            //对所有回路按照结点数量递减排序。
            //for (int i = cycleset.Count - 1; i > 0; i--)
            //{
            //    bool flag = true;
            //    List<string> temp = new List<string>();
            //    for (int j = cycleset.Count - 1; j > 0; j--)
            //        if(cycleset[j].Count > cycleset[j - 1].Count)
            //        {
            //            temp = cycleset[j];
            //            cycleset.RemoveAt(j);
            //            cycleset.Insert(j - 1, temp);
            //            flag = false;
            //        }
            //    if (flag) break;
            //}
            //删除等价回路
            //for (int i = 0; i < cycleset.Count; i++)
            //{
            //    for (int j = i + 1; j < cycleset.Count; j++)
            //    {
            //        int count = 0;
            //        if (cycleset[i].Count != cycleset[j].Count) continue;
            //        foreach (string s1 in cycleset[i])
            //            foreach (string s2 in cycleset[j])
            //                if (s1.Equals(s2))
            //                    count++;
            //        if (cycleset[j].Count == count)
            //        {
            //            cycleset.RemoveAt(j);
            //            if (j > i) j--;
            //        }
            //    }
            //}
            //根据DFS树对序列元素进行排序
            //List<State> DFStree = new List<State>();
            //m.getDFSStates(m.start, ref DFStree);
            //foreach (List<string> cycle in cycleset)
            //{
            //    foreach (State state in DFStree)
            //    {
            //        if (cycle.Contains(state.identifier))
            //        {
            //            while (!cycle[0].Equals(state.identifier))
            //            {
            //                string temp = cycle[0];
            //                cycle.RemoveAt(0);
            //                cycle.Add(temp);
            //            }
            //            break;
            //        }
            //    }
            //}
            //以结点最多的回路作为结点C进行筛选
            //for (int c = 0; c < cycleset.Count; c++)
            //{

            //    bool flag = false;
            //    int start = Int16.Parse(cycleset[c][0].Substring(1));
            //    int end = Int16.Parse(cycleset[c][cycleset[c].Count - 1].Substring(1));
            //    //直到筛选回路仅有两个结点为止
            //    if (cycleset[c].Count < 3) break;
            //    //if (cycleset[c].Count > 3)
            //    {
            //        for (int a = c + 1; a < cycleset.Count; a++)
            //        {
            //            //子序列帅选冗余回路
            //            flag = Subsquence(cycleset[c], cycleset[a]);
            //            if (flag) break;
            //        }
            //        if (flag)
            //        {
            //            cycleset.RemoveAt(c);
            //            if (c >= 0) c--;
            //            else c = -1;
            //        }
            //    }
            //使用交集与头尾相同原理筛选冗余回路
            //else
            //{
            //    //判断C的起始结点是否能直接到达其终止结点。
            //    if (Ri[start, end].Length > 1) continue;
            //    for (int a = c + 1; a < cycleset.Count; a++)
            //    {

            //        for (int b = a + 1; b < cycleset.Count; b++)
            //        {
            //            if (cycleset[a].Count < 2) continue;
            //            if (cycleset[b].Count < 2) continue;
            //            int count = 0;
            //            List<string> temp = new List<string>();
            //            //判断A和B的并集是否为C。
            //            temp = cycleset[a].Union(cycleset[b]).ToList();
            //            if (temp.Count == cycleset[c].Count)
            //            {
            //                foreach (string s1 in cycleset[c])
            //                    foreach (string s2 in temp)
            //                        if (s1.Equals(s2)) count++;
            //                //若满足上述二条件，删除回路C。
            //                if (cycleset[c].Count == count)
            //                {
            //                    cycleset.RemoveAt(c);
            //                    flag = true;
            //                    c--;
            //                    break;
            //                }
            //            }
            //        }
            //        if (flag) break;
            //    }
            //}
            //}

            return cycleset;
        }

    }
}
