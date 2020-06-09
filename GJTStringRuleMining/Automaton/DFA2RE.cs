using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//using System.Collections.Generic;

namespace MZQStringRuleMining.Automaton
{
    //包含将DFA转换成正则表达式的方法
    class DFA2RE
    {
        //采用Transitive Closure Method将DFA转换成正则表达式。
        //该方法产生的正则表达式比较冗长，无法体现出前缀树将前缀合并后的简洁性。效果不理想。
        public static string TransitiveClosureMethod(StateMachine m, List<string> order)
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

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                    Ri[x, y] = "#";//#代表空集

            int i, j;
            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));

                    if (i == j)
                        //Ri[i, j] = t.identifier.Length > 1 ? "(" + t.identifier + ")" : t.identifier + "|" + "Epsilon";//Epsilon代表空字符
                        Ri[i, j] = t.identifier.Length == 1 ? t.identifier : "!";//ϵ代表空字符
                    else Ri[i, j] = t.identifier;
                }
            }

            //DFA2RE.TCMPrintfMatrix(Ri);

            //3.迭代计算Ri和 Rj
            //指定计算序列
            int[] order_int = new int[order.Count + 2];
            int[] order_operated = new int[order.Count + 2];
            for (int pointer = 0; pointer < order.Count; pointer++)
            {
                order_int[pointer] = Convert.ToInt16(order[pointer]);
                order_operated[pointer] = -1;
            }
            //添加始态和终态
            order_int[order.Count] = 0;
            order_int[order.Count + 1] = count_States - 1;
            order_operated[order.Count] = -1;
            order_operated[order.Count + 1] = -1;

            int pointer_operated = 0;
            foreach (int z in order_int)
            {
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
                                if (Riz.Equals("#") || Rzy.Equals("#")) continue;

                                if (Rzz.Equals("#"))
                                {
                                    string Rizzy = (util.RegxHandler.canUsedAsAndOperator(Riz) ? Riz : "(" + Riz + ")") + (util.RegxHandler.canUsedAsAndOperator(Rzy) ? Rzy : "(" + Rzy + ")");
                                    if (!Rj[x, y].Equals("#")) Rj[x, y] = Rj[x, y] + "|" + Rizzy;
                                    else Rj[x, y] = Rizzy;
                                    continue;
                                }
                                else
                                {
                                    string Rizzzzy = (util.RegxHandler.canUsedAsAndOperator(Riz) ? Riz : "(" + Riz + ")") + (Rzz.Length > 1 ? "(" + Rzz + ")" : Rzz) + "*" + (util.RegxHandler.canUsedAsAndOperator(Rzy) ? Rzy : "(" + Rzy + ")");
                                    if (!Rj[x, y].Equals("#")) Rj[x, y] = Rj[x, y] + "|" + Rizzzzy;
                                    else Rj[x, y] = Rizzzzy;
                                    continue;
                                }
                            }


                //将Rj复制到Ri
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        if (!(order_operated.Contains(x) || order_operated.Contains(y))) Ri[x, y] = Rj[x, y];
                        else Ri[x, y] = "#";

                //DFA2RE.TCMPrintfMatrix(Ri);
            }
            string ex = "";

            //将每个终结符上的正则表达式用或运算连接到一起。
            foreach (State s in l_states)
                if ((s.type.Equals("E")) && (!Ri[0, Int32.Parse(s.identifier.Substring(1))].Equals("#")))
                {
                    string reg = Ri[0, Int32.Parse(s.identifier.Substring(1))];
                    ex = ex + reg + "  |  ";
                }
            return ex.Substring(0, ex.Length - 5);
        }
        //采用状态消减法（State elimination Method）将DFA转换成正则表达式。    
        //m是需要处理的自动机，order是状态消减的次序。order指定的顺序对于最终生成的正则表达式影响很大。
        //前题条件：状态机m只有一个初态和一个终态。
        public static string StateEliminationMethod(StateMachine m, List<string> labels)
        {
            //按order中状态的顺序，从前往后消减状态。
            //若消减状态q,需要的信息包括q所有前续转移，后续转移。

            List<State> order = new List<State>();

            for (int j = 0; j < labels.Count; j++)
            {
                List<State> l = new List<State>();
                order.Add(m.searchState(m.start, labels[j], ref l));
            }

            for (int i = 0; i < order.Count; i++)
            {
                m.stateList.Remove(order[i]);

                //获取自动机的状态列表。
                List<State> l_states = new List<State>();
                m.getDFSStates(m.start, ref l_states);

                State x = order[i];
                //消减状态x。
                List<State> preState = new List<State>();//x的前续状态列表
                List<Transition> self = new List<Transition>();//x的自身转移
                foreach (State s in l_states)
                {
                    foreach (Transition t in s.transitions)
                    {
                        if (t.target == x)
                        {
                            if (s != x)
                            { if (!preState.Contains(s)) preState.Add(s); }//如果存在指向x的转移，则将状态加入前续状态列表。
                            else
                            { self.Add(t); }//如果是自身转移，则加到self转移列表中。
                        }
                    }
                }
                //构造自身表达式。
                string selfexp = "";

                foreach (Transition t in self)
                {
                    if (t.identifier.Length > 1) selfexp = selfexp + "(" + t.identifier + ")*";
                    else selfexp = selfexp + t.identifier + "*";
                }

                foreach (State pres in preState)
                {
                    //去除pres指向x的转移
                    string prestring = "";
                    for (int j = pres.transitions.Count - 1; j >= 0; j--)
                    {
                        if (pres.transitions[j].target == x)
                        {
                            // prestring = prestring + pres.transitions[j].identifier + "|";
                            prestring = util.RegxHandler.Or(prestring, pres.transitions[j].identifier);
                            pres.transitions.RemoveAt(j);
                        }
                    }
                    //   prestring = prestring.Substring(0,prestring.Length-1);//去掉最后一个“|”符号。

                    foreach (Transition t in x.transitions)
                    {
                        State subs = t.target;
                        string substring = t.identifier;
                        //加入指向subs的转移
                        if (!util.RegxHandler.canUsedAsAndOperator(prestring)) prestring = "(" + prestring + ")";
                        if (!util.RegxHandler.canUsedAsAndOperator(selfexp)) selfexp = "(" + selfexp + ")";
                        if (!util.RegxHandler.canUsedAsAndOperator(substring)) substring = "(" + substring + ")";

                        string str = prestring + selfexp + substring;
                        //如果pres->subs的转移不存在，则新增，否则合并。
                        bool isFind = false;
                        foreach (Transition oldt in pres.transitions)
                        {
                            if (oldt.target == subs)
                            {
                                // oldt.identifier = oldt.identifier + "|" + str;
                                oldt.identifier = util.RegxHandler.Or(oldt.identifier, str);
                                isFind = true;
                            }
                        }
                        if (!isFind)
                        {
                            Transition newT = new Transition(str);
                            newT.target = subs;
                            pres.transitions.Add(newT);
                        }
                        pres.transitions.RemoveAll(delegate (Transition tra) { return tra.target == x; });
                    }

                }
                //Stack<State> l = new Stack<State>();
                //m.display(m.start, l);

                //SEMPrintfExpression(m.clone(), labels.Count + 2, order);  //打印过程数据
            }
            //将初态到终态的转移上的标识输出。
            string result = "";

            foreach (Transition t in m.start.transitions)
            {
                if (t.target.type == "E") result = result + t.identifier + "|";//指向终态的转移
                else if (t.target == m.start) result = (t.identifier.Length > 1 ? "(" + t.identifier + ")*" : t.identifier + "*") + //指向初态自身的转移
                    (util.RegxHandler.canUsedAsAndOperator(result) ? result : "(" + result + ")");
                else Console.WriteLine(@"DFA2RE.StateEliminationMethod异常：状态未消减完");
            }

            List<State> ends = m.getEndState();//假设可以有多个终止态。事实上在算法中是不允许有多个终态存在的。
            foreach (State end in ends)
            {
                foreach (Transition t in end.transitions)
                {
                    if (t.target == end)//指向自身的转移。
                        result = (util.RegxHandler.canUsedAsAndOperator(result) ? result : "(" + result + ")") +
                            (t.identifier.Length > 1 ? "(" + t.identifier + ")*" : t.identifier + "*");
                    else
                        Console.WriteLine(@"DFA2RE.StateEliminationMethod异常：状态未消减完或终止状态存在指向初态的情况（未处理）");
                }
            }

            if (result.Length > 0) result = result.Substring(0, result.Length - 1);
            return result;
        }

        // Brzozowski 代数法
        // 参考:https://cs.stackexchange.com/questions/2016/how-to-convert-finite-automata-to-regular-expressions
        public static string BrzozowskiAlgebraicMethod(StateMachine m, List<string> order)
        {
            //1.获取自动机状态数量。
            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            
            //2.初始化数组
            string[] B = new string[count_States];
            string[,] A = new string[count_States, count_States];
            for (int x = 0; x < count_States; x++)
                if (x == count_States - 1) B[x] = "!";  //!代表空字符
                else B[x] = "#";                        //#代表空集

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                    if (x == y) A[x, y] = "!";          //!代表空字符
                    else A[x, y] = "#";                 //#代表空集

            foreach (State s in m.stateList)
            {
                int i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    int j = Int32.Parse(t.target.identifier.Substring(1));
                    if (i == j) A[i, j] = t.identifier.Length == 1 ? t.identifier : "!";//!代表空字符
                    else A[i, j] = t.identifier;
                }
            }

            //DFA2RE.TCMPrintfMatrix(A);

            //3.迭代计算A和B
            //指定计算序列
            int[] order_int = new int[order.Count];
            int[] order_operated = new int[order.Count];
            for (int pointer = 0; pointer < order.Count; pointer++)
            {
                order_int[pointer] = Convert.ToInt16(order[pointer].Substring(1));
                order_operated[pointer] = -1;
            }

            int pointer_operated = 0;
            foreach (int z in order_int)
            {
                if (z != 0 && z != (count_States - 1))
                    order_operated[pointer_operated++] = z;

                string temp_A = A[z, z].Length > 1 ? "(" + A[z, z] + ")" : A[z, z];
                if (!A[z, z].Equals("#") && !B[z].Equals("#"))
                {
                    string temp_b = util.RegxHandler.canUsedAsAndOperator(B[z]) ? B[z] : "(" + B[z] + ")";
                    if (!A[z, z].Equals("!") && !B[z].Equals("!"))
                        B[z] = temp_A + "*" + temp_b;
                    else if (!A[z, z].Equals("!") && B[z].Equals("!"))
                        B[z] = temp_A + "*";
                }

                for (int y = 0; y < count_States; y++)
                    if (order_operated.Contains(y)) continue;
                    else
                    {
                        if (!A[z, z].Equals("#") && !A[z, y].Equals("#"))
                        {
                            string temp_a = util.RegxHandler.canUsedAsAndOperator(A[z, y]) ? A[z, y] : "(" + A[z, y] + ")";
                            if (!A[z, z].Equals("!") && !A[z, y].Equals("!"))
                                A[z, y] = temp_A + "*" + temp_a;
                            else if (!A[z, z].Equals("!") && A[z, y].Equals("!"))
                                A[z, y] = temp_A + "*";
                        }
                    }

                for (int x = 0; x < count_States; x++)
                    if (order_operated.Contains(x)) continue;
                    else
                    {

                        if (!A[x, z].Equals("#") && !B[z].Equals("#"))
                        {
                            if (!A[x, z].Equals("!") && !B[z].Equals("!"))
                            {
                                if (B[x].Equals("#") || B[x].Equals("!")) B[x] = A[x, z] + B[z];
                                else
                                {
                                    string temp_ab = "";
                                    temp_ab = util.RegxHandler.canUsedAsAndOperator(A[x, z]) ? A[x, z] : "(" + A[x, z] + ")";
                                    temp_ab += util.RegxHandler.canUsedAsAndOperator(B[z]) ? B[z] : "(" + B[z] + ")";
                                    B[x] += "|" + temp_ab;
                                }
                            }
                            else if (!A[x, z].Equals("!") && B[z].Equals("!"))
                            {
                                if (B[x].Equals("#") || B[x].Equals("!")) B[x] = A[x, z];
                                else
                                {
                                    string temp_a = "";
                                    temp_a = util.RegxHandler.canUsedAsAndOperator(A[x, z]) ? A[x, z] : "(" + A[x, z] + ")";
                                    B[x] += "|" + temp_a;
                                }
                            }
                            else if (A[x, z].Equals("!") && !B[z].Equals("!"))
                            {
                                if (B[x].Equals("#") || B[x].Equals("!")) B[x] = B[z];
                                else
                                {
                                    string temp_b = "";
                                    temp_b = util.RegxHandler.canUsedAsAndOperator(B[z]) ? B[z] : "(" + B[z] + ")";
                                    B[x] += "|" + temp_b;
                                }
                            }
                        }

                        for (int y = 0; y < count_States; y++)
                            if (order_operated.Contains(y)) continue;
                            else
                            {
                                if (!A[x, z].Equals("#") && !A[z, y].Equals("#"))
                                {
                                    if (!A[x, z].Equals("!") && !A[z, y].Equals("!"))
                                    {
                                        if (A[x, y].Equals("#") || A[x, y].Equals("!"))
                                        {
                                            string temp_a = "";
                                            temp_a = util.RegxHandler.canUsedAsAndOperator(A[x, z]) ? A[x, z] : "(" + A[x, z] + ")";
                                            temp_a += util.RegxHandler.canUsedAsAndOperator(A[z, y]) ? A[z, y] : "(" + A[z, y] + ")";
                                            A[x, y] = temp_a;
                                        }
                                        else
                                        {
                                            string temp_a = "";
                                            temp_a = util.RegxHandler.canUsedAsAndOperator(A[x, z]) ? A[x, z] : "(" + A[x, z] + ")";
                                            temp_a += util.RegxHandler.canUsedAsAndOperator(A[z, y]) ? A[z, y] : "(" + A[z, y] + ")";
                                            A[x, y] += "|" + temp_a;
                                        }
                                    }
                                    else if (!A[x, z].Equals("!") && A[z, y].Equals("!"))
                                    {
                                        if (A[x, y].Equals("#") || A[x, y].Equals("!"))
                                            A[x, y] = A[x, z];
                                        else
                                        {
                                            if (y != z)
                                            {
                                                string temp_a = "";
                                                temp_a = util.RegxHandler.canUsedAsAndOperator(A[x, z]) ? A[x, z] : "(" + A[x, z] + ")";
                                                A[x, y] += "|" + temp_a;
                                            }

                                        } 
                                    }
                                    else if (A[x, z].Equals("!") && !A[z, y].Equals("!"))
                                    {
                                        if (A[x, y].Equals("#") || A[x, y].Equals("!"))
                                            A[x, y] = A[z, y];
                                        else
                                        {
                                            if (x != z)
                                            {
                                                string temp_a = "";
                                                temp_a = util.RegxHandler.canUsedAsAndOperator(A[z, y]) ? A[z, y] : "(" + A[z, y] + ")";
                                                A[x, y] += "|" + temp_a;
                                            }
                                        }
                                    }
                                }
                            }
                    }
                //DFA2RE.TCMPrintfMatrix(A);

            }
            //if (B[0].First().Equals('(') && B[0].Last().Equals(')'))
            //{
            //    B[0] = B[0].Remove(0, 1);
            //    B[0] = B[0].Remove(B[0].Length - 1, 1);
            //}
            return B[0];
        }

        //利用字符串生成表达式需要花费较长时间，因此改用长度生成表达式
        public static double TransitiveClosureSimulate(StateMachine m, List<string> order, bool pruning, double shorterlen, ref double elimiate_count, ref List<string> error_order)
        {
            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            m.getDFSStates(m.start, ref l_states);
            //获取自动机状态数量。
            bool cutdown = false;
            int count_States = l_states.Count;
            if (!m.getEndState()[0].identifier.Substring(1).Equals(Convert.ToString(count_States - 1)))
                count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            double[,] Ri = new double[count_States, count_States];
            double[,] Rj = new double[count_States, count_States];

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                {
                    Ri[x, y] = 0;//代表空集
                    Rj[x, y] = 0;
                }

            int i, j;
            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));
                    if (t.identifier.Equals("!"))
                        Ri[i, j] = t.identifier.Length;
                    else
                    {
                        if (Regex.Match(t.identifier, @"[0-9]+").ToString().Length == 0)
                        {
                            Ri[i, j] = 1;
                        }
                        else
                        {
                            Ri[i, j] = Convert.ToInt32(t.identifier);
                        }
                    }
                        
                }
            }

            //string[,] printf = new string[count_States, count_States];
            //for (int a = 0; a < count_States; a++)
            //    for (int b = 0; b < count_States; b++)
            //        printf[a, b] = Ri[a, b].ToString();
            //DFA2RE.TCMPrintfMatrix(printf);

            //3.迭代计算Ri和 Rj
            //指定计算序列
            int[] order_int = new int[order.Count + 2];
            int[] order_operated = new int[order.Count + 2];
            for (int pointer = 0; pointer < order.Count; pointer++)
            {
                order_int[pointer] = Convert.ToInt16(order[pointer]);
                order_operated[pointer] = -1;
            }
            //添加始态和终态
            order_int[order.Count] = 0;
            order_int[order.Count + 1] = count_States - 1;
            order_operated[order.Count] = -1;
            order_operated[order.Count + 1] = -1;

            int pointer_operated = 0;
            foreach (int z in order_int)
            {
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
                                double Riz = Ri[x, z];
                                double Rzz = Ri[z, z];
                                double Rzy = Ri[z, y];
                                bool led = false;
                                if (Riz == 0 || Rzy == 0) continue;
                                if (x == z || y == z)
                                    led = true;
                                double Rizzy = Riz + Rzz + Rzy;
                                Rj[x, y] += Rizzy;
                                continue;
                            }

                //将Rj复制到Ri
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                    {
                        if (!(order_operated.Contains(x) || order_operated.Contains(y))) Ri[x, y] = Rj[x, y];
                        else Ri[x, y] = 0;
                        Rj[x, y] = 0;
                    }

                //string[,] printf = new string[count_States, count_States];
                //for (int a = 0; a < count_States; a++)
                //    for (int b = 0; b < count_States; b++)
                //        printf[a, b] = Ri[a, b].ToString();
                //DFA2RE.TCMPrintfMatrix(printf);

                if (shorterlen > 0)
                {
                    double matrix_sum = 0;
                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                            if (Ri[x, y] > 0) matrix_sum += Ri[x, y];
                    if (matrix_sum > shorterlen && pruning)
                    {
                        elimiate_count += order_int.Length - 2 - 1 - order_int.ToList().IndexOf(z);
                        for (int k = 0; order_int[k] != z; k++) error_order.Add(order_int[k].ToString());
                        error_order.Add(z.ToString());
                        return 0;
                    }

                }

            }
            double exlen = 0;

            //将每个终结符上的正则表达式用或运算连接到一起。
            foreach (State s in l_states)
                if ((s.type.Equals("E")) && (!Ri[0, Int32.Parse(s.identifier.Substring(1))].Equals("#")))
                    exlen = exlen + Ri[0, int.Parse(s.identifier.Substring(1))];

            return exlen;
        }

        public static string TCM_NaturalOrder(StateMachine m)
        {
            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            m.getDFSStates(m.start, ref l_states);
            //获取自动机状态数量。
            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            string[,] Ri = new string[count_States, count_States];
            string[,] Rj = new string[count_States, count_States];

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                    Ri[x, y] = "#";//#代表空集

            int i, j;
            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));

                    if (i == j)
                        Ri[i, j] = t.identifier.Length == 1 ? t.identifier : "!";//ϵ代表空字符
                    else Ri[i, j] = t.identifier;
                }
            }

            //DFA2RE.TCMPrintfMatrix(Ri);

            //3.迭代计算Ri和 Rj
            //指定计算序列
            int[] order_int = new int[count_States - 2];
            int[] order_operated = new int[count_States - 2];
            for (int pointer = 0; pointer < (count_States - 2); pointer++)
            {
                order_int[pointer] = pointer + 1;
                order_operated[pointer] = -1;
            }

            int pointer_operated = 0;
            foreach (int z in order_int)
            {
                order_operated[pointer_operated++] = z;
                //根据Ri计算Rj
                for (int x = 0; x < count_States; x++)
                {
                    if (order_operated.Contains(x)) continue;
                    for (int y = 0; y < count_States; y++)
                    {
                        if (order_operated.Contains(y)) continue;

                        Rj[x, y] = Ri[x, y];
                        string Riz = Ri[x, z];
                        string Rzz = Ri[z, z];
                        string Rzy = Ri[z, y];
                        if (Riz.Equals("#") || Rzy.Equals("#")) continue;

                        string Rizzy;
                        if (Rzz.Equals("#"))
                        {
                            Rizzy = (util.RegxHandler.canUsedAsAndOperator(Riz) ? Riz : "(" + Riz + ")") + (util.RegxHandler.canUsedAsAndOperator(Rzy) ? Rzy : "(" + Rzy + ")");
                        }
                        else
                        {
                            Rizzy = (util.RegxHandler.canUsedAsAndOperator(Riz) ? Riz : "(" + Riz + ")") + (Rzz.Length > 1 ? "(" + Rzz + ")" : Rzz) + "*" + (util.RegxHandler.canUsedAsAndOperator(Rzy) ? Rzy : "(" + Rzy + ")");
                        }
                        if (!Rj[x, y].Equals("#"))
                        {
                            int for_idx = 0, back_idx = 0;
                            int for_brackets_addr = 0, back_brackets_addr = 0;
                            int for_record = 0, back_record = 0, for_no_operat = 0, back_no_operat = 0;
                            int jlen = Rj[x, y].Length - 1, ylen = Rizzy.Length - 1;
                            for (int check_brackets = 0; Rj[x, y][for_idx] == Rizzy[for_idx]; for_idx++)
                            {
                                if (Regex.IsMatch(Rj[x, y][for_idx].ToString(), "[a-zA-Z]")) for_no_operat++;
                                if (Rj[x, y][for_idx] == '(') check_brackets++;
                                if (Rj[x, y][for_idx] == ')')
                                {
                                    check_brackets--;
                                    if (check_brackets == 0)
                                    {
                                        for_brackets_addr = for_idx;
                                        for_record = for_no_operat;
                                        if (for_idx + 1 <= jlen) 
                                            if (Rj[x, y][for_idx + 1] == '*')
                                            {
                                                for_brackets_addr++;
                                                for_record++;
                                            }
                                    }
                                }
                            }
                            for (int check_brackets = 0; Rj[x, y][jlen - back_idx] == Rizzy[ylen - back_idx]; back_idx++)
                            {
                                if (Regex.IsMatch(Rj[x, y][jlen - back_idx].ToString(), "[a-zA-Z]")) back_no_operat++;
                                if (Rj[x, y][jlen - back_idx] == ')') check_brackets--;
                                if (Rj[x, y][jlen - back_idx] == '(')
                                {
                                    check_brackets++;
                                    if (check_brackets == 0)
                                    {
                                        back_brackets_addr = back_idx;
                                        back_record = back_no_operat;
                                    }
                                }
                                
                            }

                            if (for_idx > for_brackets_addr) 
                            { 
                                for_idx = for_brackets_addr; 
                                for_no_operat = for_record; 
                            }
                            if (back_idx > back_brackets_addr)
                            {
                                back_idx = back_brackets_addr;
                                back_no_operat = back_record;
                            }

                            if (for_no_operat > 0 || back_no_operat > 0)
                            {
                                string for_substr = Rj[x, y].Substring(0, for_idx);
                                string back_substr = Rj[x, y].Substring(jlen - back_idx, back_idx + 1);
                                string Rj_mid = Rj[x, y].Substring(for_idx, jlen - back_idx - for_idx);
                                string zzy_mid = Rizzy.Substring(for_idx, ylen - back_idx - for_idx);
                                Rj[x, y] = string.Format("{0}({1}|{2}){3}", for_substr, Rj_mid, zzy_mid, back_substr);
                            }
                            else Rj[x, y] = Rj[x, y] + "|" + Rizzy;
                        }
                        else Rj[x, y] = Rizzy;
                    }
                        
                }
                    
                //将Rj复制到Ri
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        if (!(order_operated.Contains(x) || order_operated.Contains(y))) Ri[x, y] = Rj[x, y];
                        else Ri[x, y] = "#";

                DFA2RE.TCMPrintfMatrix(Ri);
            }

            return Ri[0, count_States - 1];
        }

        //采用Transitive Closure Method将DFA转换成正则表达式。
        //该方法产生的正则表达式比较冗长，无法体现出前缀树将前缀合并后的简洁性。效果不理想。
        public static StateMachine DFA2EFA(StateMachine m, string state)
        {
            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            m.getDFSStates(m.start, ref l_states);
            //获取自动机状态数量。
            int count_States = l_states.Count;
            if (!m.getEndState()[0].identifier.Substring(1).Equals(Convert.ToString(count_States - 1)))
                count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            double[,] Ri = new double[count_States, count_States];
            double[,] Rj = new double[count_States - 1, count_States - 1];

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                {
                    Ri[x, y] = 0;//代表空集
                    if ((x < count_States - 1) && (y < count_States - 1))
                        Rj[x, y] = 0;
                }

            int i, j;
            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));
                    if (t.identifier.Equals("!"))
                        Ri[i, j] = t.identifier.Length;
                    else
                        Ri[i, j] = Convert.ToInt32(t.identifier);
                }
            }

            //string[,] printf = new string[count_States, count_States];
            //for (int a = 0; a < count_States; a++)
            //    for (int b = 0; b < count_States; b++)
            //        printf[a, b] = Ri[a, b].ToString();
            //DFA2RE.TCMPrintfMatrix(printf);

            //3.迭代计算Ri和 Rj
            //指定计算序列
            //根据Ri计算Rj
            int z = Convert.ToInt32(state);
            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                {
                    double Riz = Ri[x, z];
                    double Rzz = Ri[z, z];
                    double Rzy = Ri[z, y];

                    if (x > z && y > z) Rj[x - 1, y - 1] = Ri[x, y];
                    else if (x < z && y > z) Rj[x, y - 1] = Ri[x, y];
                    else if (x > z && y < z) Rj[x - 1, y] += Ri[x, y];
                    else if (x < z && y < z) Rj[x, y] += Ri[x, y];

                    if (Riz == 0 || Rzy == 0) continue;
                    if (x == z || y == z) continue;

                    double Rizzy = Riz + Rzz + Rzy;
                    if (x > z && y > z) Rj[x - 1, y - 1] += Rizzy;
                    else if (x < z && y > z) Rj[x, y - 1] += Rizzy;
                    else if (x > z && y < z) Rj[x - 1, y] += Rizzy;
                    else if (x < z && y < z) Rj[x, y] += Rizzy;
                }

            //string[,] printf = new string[count_States, count_States];
            //for (int a = 0; a < count_States; a++)
            //    for (int b = 0; b < count_States; b++)
            //        printf[a, b] = Ri[a, b].ToString();
            //DFA2RE.TCMPrintfMatrix(printf);

            //重构自动机
            StateMachine efa = new StateMachine();
            efa.GenerateEFAbyMatrix(Rj);

            return efa;
        }

        //在传递闭包法中打印消减过程中的转移矩阵
        public static void TCMPrintfMatrix(string[,] array)
        {
            for (int b = 0; b < array.GetLength(1); b++)
                Console.Write("\t" + "State" + b);
            Console.WriteLine();
            for (int a = 0; a < array.GetLength(0); a++)
            {
                Console.Write("State" + a + "\t");
                for (int b = 0; b < array.GetLength(1); b++)
                    Console.Write(array[a, b] + "\t");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //在状态消减法中打印消减过程中的转移矩阵
        public static void SEMPrintfExpression(StateMachine m, int labels_Count, List<State> order)
        {
            Console.WriteLine();
            Console.WriteLine();
            bool flag_x = false;
            bool flag_y = false;
            string expression = "";

            for (int states = 0; states < labels_Count; states++)
                Console.Write("\t" + "State" + states);
            Console.WriteLine();

            Console.Write("M0\t");  //M0的转移关系需要单独处理
            for (int counter_x = 0; counter_x < labels_Count; counter_x++)
            {
                foreach (Transition t in m.stateList[0].transitions)
                {
                    if (t.target.identifier.Substring(1).Equals(counter_x.ToString()))
                    {
                        expression = t.identifier;
                        flag_x = true;
                    }
                }

                if (flag_x)
                {
                    Console.Write(expression + "\t");
                    flag_x = false;
                }
                else Console.Write("\t");
            }
            Console.WriteLine();

            for (int counter_y = 1; counter_y < labels_Count; counter_y++)
            {
                Console.Write("M" + counter_y + "\t");
                foreach (State s in order)
                {
                    if (s.identifier.Substring(1).Equals(counter_y.ToString()))
                    {
                        for (int counter_x = 0; counter_x < labels_Count; counter_x++)
                        {
                            foreach (Transition t in s.transitions)
                            {
                                if (t.target.identifier.Substring(1).Equals(counter_x.ToString()))
                                {
                                    expression = t.identifier;
                                    flag_x = true;
                                }
                            }

                            if (flag_x)
                            {
                                Console.Write(expression + "\t");
                                flag_x = false;
                            }
                            else Console.Write("\t");

                        }
                        flag_y = true;
                        Console.WriteLine();
                        break;
                    }
                }

                if (flag_y) flag_y = false;
                else
                {
                    for (int counter_x = 0; counter_x < labels_Count; counter_x++)
                    {
                        Console.Write("\t");
                    }
                    Console.WriteLine();
                }

            }
        }

        public static double[,] DFA2Matrix(StateMachine m)
        {

            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            m.getDFSStates(m.start, ref l_states);
            //获取自动机状态数量。
            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            double[,] Ri = new double[count_States, count_States];

            for (int x = 0; x < count_States; x++)
                for (int y = 0; y < count_States; y++)
                    Ri[x, y] = 0;

            int i, j;
            foreach (State s in l_states)
            {
                i = Int32.Parse(s.identifier.Substring(1));
                foreach (Transition t in s.transitions)
                {
                    j = Int32.Parse(t.target.identifier.Substring(1));
                    if (t.identifier.Equals("!"))
                        Ri[i, j] = t.identifier.Length;
                    else
                        Ri[i, j] = Convert.ToInt32(t.identifier);
                }
            }

            return Ri;
        }
        public static double[,] TCM_AState(double[,] Ri, int target_state)
        {
            //string[,] printf = new string[count_States, count_States];
            //for (int a = 0; a < count_States; a++)
            //    for (int b = 0; b < count_States; b++)
            //        printf[a, b] = Ri[a, b].ToString();
            //DFA2RE.TCMPrintfMatrix(printf);
            double[,] Rj = new double[Ri.GetLength(0), Ri.GetLength(0)];
            for (int x = 0; x < Ri.GetLength(0); x++)
                for (int y = 0; y < Ri.GetLength(0); y++)
                    Rj[x, y] = 0;

            //3.迭代计算Ri和 Rj
            int z = target_state;
            for (int x = 0; x < Ri.GetLength(0); x++)
                for (int y = 0; y < Ri.GetLength(0); y++)
                    if (x == z || y == z) continue;
                    else
                    {
                        Rj[x, y] = Ri[x, y];
                        if (Ri[x, z] == 0 || Ri[z, y] == 0) continue;
                        Rj[x, y] += (Ri[x, z] + Ri[z, z] + Ri[z, y]);
                    }

            //string[,] printf = new string[count_States, count_States];
            //for (int a = 0; a < count_States; a++)
            //    for (int b = 0; b < count_States; b++)
            //        printf[a, b] = Ri[a, b].ToString();
            //DFA2RE.TCMPrintfMatrix(printf);

            return Rj;
        }

    }
}