using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class SecondMethod
    {
        // 输出一个数组的全排列
        public static List<string> DynamicEliminationMethod(StateMachine machine, string FuncK)
        {
            //0、初始化最佳状态消减序列
            List<string> best_order = new List<string>();
            //1.获取自动机的状态列表。
            List<State> l_states = new List<State>();
            machine.getDFSStates(machine.start, ref l_states);
            //获取自动机状态数量。
            int count_States = l_states.Count;
            if (!machine.getEndState()[0].identifier.Substring(1).Equals(Convert.ToString(count_States - 1)))
                count_States = Convert.ToInt16(machine.getEndState()[0].identifier.Substring(1)) + 1;
            //2.初始化R数组
            double[,] Ri = new double[count_States, count_States];
            double[,] Rj = new double[count_States, count_States];
            double[] z = new double[2] { -1, -1 };

            for (int x = 0; x < count_States; x++) for (int y = 0; y < count_States; y++) Ri[x, y] = 0;//#代表空集

            foreach (State s in l_states)
                foreach (Transition t in s.transitions)
                {
                    int j = Int32.Parse(t.target.identifier.Substring(1));
                    Ri[Int32.Parse(s.identifier.Substring(1)), j] = t.identifier.Length;
                }

            //Algorithm.TCMPrintfMatrix(Ri);

            //3.迭代计算Ri和 Rj
            for (int i = 0; i < l_states.Count - 2; i++)  //消减次数,次数根据自动机的实际状态数目而定
            {
                int log_int = 0;

                if (FuncK.Equals("constant")) log_int = l_states.Count - 3 - i >= 2 ? 2 : l_states.Count - 3 - i;
                else if (FuncK.Equals("cubic_root")) log_int = Convert.ToInt32(Math.Pow(l_states.Count - 3 - i, 1.0 / 3));
                else if (FuncK.Equals("square_root")) log_int = Convert.ToInt32(Math.Pow(l_states.Count - 3 - i, 1.0 / 2));
                else if (FuncK.Equals("square_root_plus_one")) log_int = Convert.ToInt32(Math.Pow(l_states.Count - 2 - i, 1.0 / 2));
                else if (FuncK.Equals("root")) log_int = Convert.ToInt32(Math.Pow(l_states.Count - 2 - i, 2.0 / 3));

                int depth = log_int > 0 ? log_int - 1 : 0;

                List<double[]> test = RecursionAlgorithm(Ri, depth);
                z = test[0];
                //Console.Write(log_int + "\t");
                //算法：搜索无关状态
                if (z[0] == 0 || z[0] == count_States - 1)  //递归算法计算出初态或终态时，表明自动机存在无关状态
                    foreach (State s in l_states)                   //搜索无关状态
                        if (best_order.Contains(s.identifier))       //保证序列的最后两位是初态和终态
                            if (!s.identifier.Equals("S0") || !s.identifier.Equals("E" + (count_States - 1)))
                                z[0] = Convert.ToInt16(s.identifier.Substring(1));

                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                    {
                        Rj[x, y] = Ri[x, y];
                        double Riz = Ri[x, (int)Math.Round(z[0],0)];
                        double Rzz = Ri[(int)Math.Round(z[0], 0), (int)Math.Round(z[0], 0)];
                        double Rzy = Ri[(int)Math.Round(z[0], 0), y];
                        if (x == z[0] || y == z[0]) continue;
                        if (Riz == 0 || Rzy == 0) continue;

                        if (Rzz == 0)
                        {
                            double Rizzy = Riz + Rzy;
                            if (Rj[x, y] != 0) Rj[x, y] = Rj[x, y] + Rizzy;
                            else Rj[x, y] = Rizzy;
                        }
                        else
                        {
                            double Rizzzzy = Riz + Rzz + Rzy;
                            if (Rj[x, y] != 0) Rj[x, y] = Rj[x, y] + Rizzzzy;
                            else Rj[x, y] = Rizzzzy;
                        }
                    }

                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        if (x == z[0] || y == z[0]) Rj[x, y] = 0;     //消减冗余信息

                foreach (State s in l_states)
                    if (s.identifier.Equals("S" + z[0])) best_order.Add(z[0].ToString());
                    else if (s.identifier.Equals("M" + z[0])) best_order.Add(z[0].ToString());
                    else if (s.identifier.Equals("E" + z[0])) best_order.Add(z[0].ToString());

                //将Rj复制到Ri
                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        Ri[x, y] = Rj[x, y];

                for (int x = 0; x < count_States; x++)
                    for (int y = 0; y < count_States; y++)
                        Rj[x, y] = 0;
            }
            foreach (State s in l_states)
                if (!best_order.Contains(s.identifier))
                    if (!s.identifier.Equals("S0") && !s.identifier.Equals("E" + (int)(count_States - 1)))
                        best_order.Add(s.identifier.Substring(1));
            best_order.Add("0");
            best_order.Add((count_States - 1).ToString());
            return best_order;
        }

        //有限深度搜索，向前走depth步寻找最优值
        public static List<double[]> RecursionAlgorithm(double[,] input_matrix, int depth)
        {
            List<double[]> best = new List<double[]> ();
            List<double[]> temp_snl = new List<double[]> { new double[2] { 0, 0 } };
            double[] weight = new double[input_matrix.GetLength(1)];//n*2矩阵，第一列用于传址，第二列用于传值
            double[,] output_matrix = new double[input_matrix.GetLength(1), input_matrix.GetLength(1)];

            for (int x = 0; x < input_matrix.GetLength(1); x++) { weight[x] = 0; }
            for (int x = 0; x < input_matrix.GetLength(1); x++)
                for (int y = 0; y < input_matrix.GetLength(1); y++)
                    output_matrix[x, y] = 0;

            //计算每个状态的权重
            for (int z = 1; z <= input_matrix.GetLength(1) - 2; z++)
            {
                bool flag = false;
                double state_length = 0;
                for (int y = 0; y < input_matrix.GetLength(1); y++)
                    if (input_matrix[z, y] != 0) flag = true;
                if (flag == false) continue;

                for (int x = 0; x < input_matrix.GetLength(1); x++)
                {
                    for (int y = 0; y < input_matrix.GetLength(1); y++)
                    {
                        output_matrix[x, y] = input_matrix[x, y];
                        double Rxz = input_matrix[x, z];
                        double Rzz = input_matrix[z, z];
                        double Rzy = input_matrix[z, y];
                        if (x == z || y == z) continue;
                        if (Rxz == 0 || Rzy == 0) continue;

                        if (Rzz == 0)
                        {
                            double Rizzy = Rxz + Rzy; ;
                            if (output_matrix[x, y] != 0) output_matrix[x, y] = output_matrix[x, y] + Rizzy;
                            else output_matrix[x, y] = Rizzy;
                            continue;
                        }
                        else
                        {
                            double Rizzzzy = Rxz + Rzz + Rzy;
                            if (output_matrix[x, y] != 0) output_matrix[x, y] = output_matrix[x, y] + Rizzzzy;
                            else output_matrix[x, y] = Rizzzzy;
                            continue;
                        }
                    }
                }

                for (int x = 0; x < input_matrix.GetLength(1); x++)
                    for (int y = 0; y < input_matrix.GetLength(1); y++)
                    {
                        if (x == z || y == z) output_matrix[x, y] = 0;  //消减冗余信息
                        state_length += output_matrix[x, y];    //统计转接矩阵长度
                    }

                //string[,] printf_matrix = new string[output_matrix.GetLength(1), output_matrix.GetLength(1)];
                //for (int x = 0; x < output_matrix.GetLength(1); x++)
                //    for (int y = 0; y < output_matrix.GetLength(1); y++)
                //        printf_matrix[x, y] = output_matrix[x, y].ToString();
                //Console.WriteLine("正在消除" + z + "号状态, 字符总数为" + state_length);
                //DFA2RE.TCMPrintfMatrix(printf_matrix);

                if (depth < 1) { weight[z] = state_length; }
                else
                {
                    temp_snl = RecursionAlgorithm(output_matrix, depth - 1);
                    weight[z] = temp_snl.Count > 0 ? temp_snl[0][1] : 0;
                }
            }

            double shortest = weight.Max() > 0 ? weight.Where(u => u > 0).Min() : 0;
            for (int i = 1; i < input_matrix.GetLength(1) - 1; i++)
            {
                if (shortest > 0 && weight[i] == shortest)
                    best.Add(new double[2] { i, shortest });                    
            }
            return best;
        }

        public static List<List<string>> SecondAdvance(StateMachine machine)
        {
            //0. 初始化
            double shortest = 0;
            List<double> price = new List<double>();                              //存储值
            List<State> l_states = new List<State>();                       //按DFS序列存储结点
            List<double[,]> matrixlist = new List<double[,]>();             //以队列结构存储转移矩阵
            List<List<string>> orderlist = new List<List<string>>();        //以队列结构存储对应转移矩阵的消减序列
            List<List<string>> shortest_orders = new List<List<string>>();

            machine.getDFSStates(machine.start, ref l_states);
            int count_States = Convert.ToInt16(machine.getEndState()[0].identifier.Substring(1)) + 1;
            double ancestor_price = 0;
            double[] in_degree = new double[count_States];
            double[] out_degree = new double[count_States];
            double[] loop = new double[count_States];

            double[,] ancestor = new double[count_States, count_States];
            for (int x = 0; x < count_States; x++) for (int y = 0; y < count_States; y++) ancestor[x, y] = 0;
            for (int x = 0; x < count_States; x++) { in_degree[x] = 0; out_degree[x] = 0; loop[x] = 0; }
            foreach (State s in machine.stateList)
                foreach (Transition t in s.transitions)
                {
                    int start = int.Parse(s.identifier.Substring(1));
                    int end = int.Parse(t.target.identifier.Substring(1));
                    ancestor[start, end] = t.identifier.Length;
                    if (start != end)
                    {
                        in_degree[end]++;
                        out_degree[start]++;
                    }
                    else loop[start]++;
                }
            for (int x = 1; x < count_States - 1; x++)
                if (in_degree[x] == 0 || out_degree[x] == 0)
                {
                    in_degree[x] = 0;
                    out_degree[x] = 0;
                    loop[x] = 0;
                    for (int y = 0; y < count_States; y++)
                    {
                        ancestor[x, y] = 0;
                        ancestor[y, x] = 0;
                    }
                }
            ancestor_price = in_degree.Sum() + out_degree.Sum() + loop.Sum(); //自动机度的总和等于字符长度总和加上边的总数
            price.Add(ancestor_price);
            orderlist.Add(new List<string>() { "0" }.ToList());
            matrixlist.Add((double[,])ancestor.Clone());
            shortest = ancestor_price;
            shortest_orders.Add(new List<string>() { "0" }.ToList());

            //string[,] printf_matrix = new string[count_States, count_States];
            //for (int x = 0; x < count_States; x++)
            //    for (int y = 0; y < count_States; y++)
            //        printf_matrix[x, y] = matrixlist[0][x, y].ToString();
            //Console.WriteLine("\n当前矩阵");
            //DFA2RE.TCMPrintfMatrix(printf_matrix);

            //BFS搜索开始
            while (price.Count > 0)
            {
                double[,] Ri = (double[,])matrixlist[0].Clone();
                int log_int = (int)Math.Floor(Math.Log(l_states.Count - orderlist[0].Count) / Math.Log(2));
                int depth = log_int > 0 ? log_int - 1 : 0;

                List<double[]> candidate = RecursionAlgorithm(Ri, depth);

                foreach (double[] z in candidate)
                {
                    bool isvalue = false;
                    int int_z = (int)Math.Round(z[0], 0);
                    double current_price = 0, parent_count = 0;
                    List<string> current_order = orderlist[0].ToList();
                    double[,] Rj = new double[count_States, count_States];
                    for (int x = 0; x < count_States; x++)
                        if (Ri[x, int_z] != 0 || Ri[int_z, x] != 0)
                            isvalue = true;
                    if (!isvalue) continue;

                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                        {
                            Rj[x, y] = Ri[x, y];
                            double Riz = Ri[x, int_z];
                            double Rzz = Ri[int_z, int_z];
                            double Rzy = Ri[int_z, y];
                            if (x == int_z || y == int_z) continue;
                            if (Riz == 0 || Rzy == 0) continue;

                            if (Rzz == 0)
                            {
                                double Rizzy = Riz + Rzy;
                                if (Rj[x, y] != 0) Rj[x, y] = Rj[x, y] + Rizzy;
                                else Rj[x, y] = Rizzy;
                            }
                            else
                            {
                                double Rizzzzy = Riz + Rzz + Rzy;
                                if (Rj[x, y] == 0) Rj[x, y] = Rj[x, y] + Rizzzzy;
                                else Rj[x, y] = Rizzzzy;
                            }
                        }

                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                            if (x == int_z || y == int_z) Rj[x, y] = 0;     //消减冗余信息

                    current_order.Add(int_z.ToString());

                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                            current_price += Rj[x, y];

                    //打印信息
                    //for (int x = 0; x < count_States; x++)
                    //    for (int y = 0; y < count_States; y++)
                    //        printf_matrix[x, y] = Rj[x, y].ToString();
                    //Console.WriteLine("当前权重：\t{0}", current_price);
                    //Console.Write("当前序列：\t");
                    //for (int x = 1; x < current_order.Count; x++)
                    //    Console.Write(current_order[x] + "\t");
                    //Console.WriteLine("\n当前矩阵");
                    //DFA2RE.TCMPrintfMatrix(printf_matrix);

                    for (int i = 0; i < price.Count; i++)
                        if (orderlist[i].Count > orderlist[0].Count)
                        {
                            if (current_price < price[i])
                            {
                                price.RemoveAt(i); //flag
                                orderlist.RemoveAt(i);
                                matrixlist.RemoveAt(i);
                                i--;
                            }
                            else if (current_price > price[i]) isvalue = false;
                        }
                        else parent_count++;
                    if (parent_count == orderlist.Count || isvalue)
                    {
                        price.Add(current_price);
                        orderlist.Add(current_order.ToList());
                        matrixlist.Add((double[,])Rj.Clone());
                        //记录算法最优集
                        if (current_order.Count > shortest_orders[0].Count)
                        {   //当计算序列中的状态数大于最优集中序列的状态数时，更新所有数据
                            shortest_orders.Clear();
                            shortest = current_price;
                            shortest_orders.Add(current_order.ToList());
                        }
                        else if (current_order.Count == shortest_orders[0].Count)
                        {   //当计算序列中的状态数等于最优集中序列的状态数
                            if (shortest > current_price)
                            {   //若计算权重小于现存最小值，更新所有数据
                                shortest_orders.Clear();
                                shortest = current_price;
                                shortest_orders.Add(current_order.ToList());
                            }   //若计算权重等于现存最小值，将计算序列加至最优集
                            else if (shortest == current_price)
                                shortest_orders.Add(current_order.ToList());
                        }
                    }
                    //Algorithm.TCMPrintfMatrix(Rj);
                }

                price.RemoveAt(0);
                orderlist.RemoveAt(0);
                matrixlist.RemoveAt(0);
            }

            for (int i = 0; i < shortest_orders.Count; i++)
            {
                shortest_orders[i].RemoveAt(0);
                shortest_orders[i].Add("0");
                shortest_orders[i].Add((count_States - 1).ToString());
            }

            return shortest_orders;
        }
    }
}
