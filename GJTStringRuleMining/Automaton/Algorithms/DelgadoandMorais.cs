using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MZQStringRuleMining.Automaton
{
    class DelgadoandMorais
    {

        //获得状态消减序列，给每个状态赋权重。
        //权重公式参考论文：Approximation to the Smallest Regular Expression for a Given Regular Language--2005
        public static List<int> getStateEliminationSequenceBasedonWeight(StateMachine m_weight)
        {
            Hashtable weight = new Hashtable();
            Hashtable weight_in = new Hashtable();
            Hashtable weight_out = new Hashtable();
            Hashtable weight_loop = new Hashtable();
            Hashtable degree_in = new Hashtable();
            Hashtable degree_out = new Hashtable();
            List<State> states = new List<State>();

            m_weight.getDFSStates(m_weight.start, ref states);
            //初始化状态与权重的键值对哈稀表。
            foreach (State s in states)
            {
                if (s.type != "S" && s.type != "E")
                {
                    weight.Add(s, 0);
                    weight_in.Add(s, 0);
                    weight_out.Add(s, 0);
                    weight_loop.Add(s, 0);
                    degree_in.Add(s, 0);
                    degree_out.Add(s, 0);
                }
            }
            //计算度
            foreach (State s in states)
            {
                foreach (Transition t in s.transitions)
                {
                    if (s.type.Equals("S") || s.type.Equals("E"))
                    {
                        if (!t.target.type.Equals("S") && !t.target.type.Equals("E"))
                            degree_in[t.target] = (int)degree_in[t.target] + 1;
                    }
                    else
                    {
                        if (t.target.type.Equals("S") || t.target.type.Equals("E"))
                            degree_out[s] = (int)degree_out[s] + 1;
                        else
                        {
                            if (!t.target.identifier.Equals(s.identifier))
                            {
                                degree_in[t.target] = (int)degree_in[t.target] + 1;
                                degree_out[s] = (int)degree_out[s] + 1;
                            }
                        }
                    }
                }
            }
            //计算权重
            foreach (State s in states)
            {
                //int in_Transition_weight = 0;
                //int out_transition_weight = 0;
                foreach (Transition t in s.transitions)
                {
                    int identifierLenght = (Regex.IsMatch(t.identifier, "[1-9]")) ? Convert.ToInt32(t.identifier) : 1;
                    if (s.type.Equals("S") || s.type.Equals("E"))
                    {
                        if (t.target != s && !t.target.type.Equals("E"))
                        {
                            int t_in = identifierLenght * ((int)degree_out[t.target] > 0 ? (int)degree_out[t.target] - 1 : 0);
                            weight_in[t.target] = (int)weight_in[t.target] + t_in;
                        }
                    }
                    else
                    {
                        int t_in = 0;
                        int t_out = identifierLenght * ((int)degree_in[s] > 0 ? (int)degree_in[s] - 1 : 0);
                        int t_loop = identifierLenght * ((int)degree_in[s] * (int)degree_out[s] > 0 ? (int)degree_in[s] * (int)degree_out[s] - 1 : 0);
                        if (t.target != s) weight_out[s] = (int)weight_out[s] + t_out;
                        if (t.target.type != "S" && t.target.type != "E")
                        {
                            t_in = identifierLenght * ((int)degree_out[t.target] > 0 ? (int)degree_out[t.target] - 1 : 0);
                            if (t.target != s)
                                weight_in[t.target] = (int)weight_in[t.target] + t_in;
                            else weight_loop[s] = (int)weight_loop[s] + t_loop;
                        }
                    }
                }
            }
            foreach (State s in states)
            {
                if (s.type == "S" || s.type == "E") continue;
                weight[s] = (int)weight[s] + (int)weight_in[s] + (int)weight_out[s] + (int)weight_loop[s];
            }
            //foreach (State s in states)
            //{
            //    Console.Write(s.identifier + " ");
            //    Console.WriteLine(weight[s]);
            //}
            //排序：按权重从小到大排序
            //按状态编号排序
            State[] ss = new State[weight.Count];
            int[] values = new int[weight.Count];
            weight.Keys.CopyTo(ss, 0);
            weight.Values.CopyTo(values, 0);
            int[] output = new int[Convert.ToInt16(m_weight.getEndState()[0].identifier.Substring(1)) + 1];

            for (int i = 0; i < output.Length; i++)
            {
                bool isfind = false;
                for (int j = 0; j < ss.Length; j++)
                {
                    if (ss[j].identifier.Substring(1).Equals(i.ToString()))
                    {
                        isfind = true;
                        output[i] = values[j];
                        break;
                    }
                }
                if (!isfind) output[i] = 0;
            }
            return output.ToList();
        }

        //利用字符串生成表达式需要花费较长时间，因此改用长度生成表达式
        public static string DynamicDMandTCMsim(StateMachine m)
        {
            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            m.getDFSStates(m.start, ref l_states);
            //获取自动机状态数量。
            int count_States = l_states.Count;
            if (!m.getEndState()[0].identifier.Substring(1).Equals(Convert.ToString(count_States - 1)))
                count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            string[,] Ri = new string[count_States, count_States];
            string[,] Rj = new string[count_States, count_States];

            int i, j;
            for (int x_idx = 0; x_idx < count_States; x_idx++)
                for (int y_idx = 0; y_idx < count_States; y_idx++)
                    Ri[x_idx, y_idx] = "";

            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));
                    Ri[i, j] = t.identifier;
                }
            }

            //3.迭代计算Ri和 Rj
            //指定计算序列
            int[] order_operated = new int[count_States];
            for (int pointer = 0; pointer < count_States; pointer++)
            {
                order_operated[pointer] = -1;
            }

            int pointer_operated = 0;
            for (int idx = 0; idx < l_states.Count - 2; idx++)
            {
                int z = DynamicDM(Ri);
                if (z != 0 && z != (count_States - 1))
                    order_operated[pointer_operated++] = z;
                //根据Ri计算Rj
                for (int x = 0; x < count_States; x++)
                    if (order_operated.Contains(x)) continue;
                    else
                        for (int y = 0; y < count_States; y++)
                            if (order_operated.Contains(y)) continue;
                            else
                            {
                                Rj[x, y] = Ri[x, y];
                                string Riz = Ri[x, z];
                                string Rzz = Ri[z, z];
                                string Rzy = Ri[z, y];
                                if (Riz.Length == 0 || Rzy.Length == 0) continue;
                                string Rizzy = Riz + Rzz + Rzy;
                                Rj[x, y] += Rizzy;
                                continue;
                            }

                //将Rj复制到Ri
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                    {
                        if (!(order_operated.Contains(x) || order_operated.Contains(y))) Ri[x, y] = Rj[x, y];
                        else Ri[x, y] = "";
                        Rj[x, y] = "";
                    }

            }
            string exlen = "";

            //将每个终结符上的正则表达式用或运算连接到一起。
            foreach (State s in l_states)
                if ((s.type.Equals("E")) && (!Ri[0, Int32.Parse(s.identifier.Substring(1))].Equals("#")))
                    exlen = exlen + Ri[0, int.Parse(s.identifier.Substring(1))];

            return exlen;
        }

        public static int DynamicDM(string[,] m_maxtrix)
        {
            int times = m_maxtrix.GetLength(0);
            double[] weight = new double[times];
            double[] weight_in = new double[times];
            double[] weight_out = new double[times];
            double[] weight_loop = new double[times];
            double[] degree_in = new double[times];
            double[] degree_out = new double[times];

            //计算度及转移权重和
            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < times; j++)
                {
                    if (i == j && m_maxtrix[i, j].Length > 0) { weight_loop[i] = m_maxtrix[i, j].Length; continue; }
                    degree_in[i] += m_maxtrix[j, i].Length > 0 ? 1 : 0;
                    degree_out[i] += m_maxtrix[i, j].Length > 0 ? 1 : 0;
                    weight_in[i] += m_maxtrix[j, i].Length;
                    weight_out[i] += m_maxtrix[i, j].Length;
                }
            }
            //权重函数计算权重
            for (int idx = 1; idx < times - 1; idx++)
            {
                weight[idx] += (degree_in[idx] > 0 ? (degree_in[idx] - 1) : 0) * weight_out[idx];
                weight[idx] += (degree_out[idx] > 0 ? (degree_out[idx] - 1) : 0) * weight_in[idx];
                weight[idx] += (degree_in[idx] > 0 && degree_out[idx] > 0 ? (degree_in[idx] * degree_out[idx] - 1) : 0) * weight_loop[idx];
                if (degree_in[idx] == 0 && degree_out[idx] == 0) weight[idx] = double.MaxValue;
            }
               
            weight[0] = double.MaxValue;
            weight[times - 1] = double.MaxValue;
            
            return Array.IndexOf(weight, weight.Min());
        }

        //获得状态消减序列，根据每个状态的出度和入度的总和计算对状态进行排序。按从小到大排序。
        

    }
}
