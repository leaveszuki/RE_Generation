using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class TimesDegree
    {
        //流量计算法
        public static List<int> StaticInDegree(StateMachine m)
        {
            List<string> output = new List<string>();
            int state_number = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            int[] product = new int[state_number];
            int[,] degree = new int[state_number, 2];//0行为出度，1行为入度
            for (int i = 0; i < state_number; i++)
            {
                product[i] = 0;
                degree[i, 0] = 0;
                degree[i, 1] = 0;
            }
            foreach (State s in m.stateList)
            {
                int out_state_num = Convert.ToInt16(s.identifier.Substring(1));
                degree[out_state_num, 0] = s.transitions.Count;
                foreach (Transition t in s.transitions)
                {
                    int in_state_num = Convert.ToInt16(t.target.identifier.Substring(1));
                    degree[in_state_num, 1]++;
                }
            }
            for (int i = 0; i < state_number; i++) product[i] = degree[i, 1];
            return product.ToList();
        }

        public static List<string> Traffic(StateMachine m)
        {
            List<string> output = new List<string>() { "M1" };
            int state_number = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            int[] product = new int[state_number];
            int[,] degree = new int[state_number, 2];//0行为出度，1行为入度
            for (int i = 0; i < state_number; i++)
            {
                product[i] = 0;
                degree[i, 0] = 0;
                degree[i, 1] = 0;
            }
            foreach (State s in m.stateList)
            {
                int out_state_num = Convert.ToInt16(s.identifier.Substring(1));
                degree[out_state_num, 0] = s.transitions.Count;
                foreach (Transition t in s.transitions)
                {
                    int in_state_num = Convert.ToInt16(t.target.identifier.Substring(1));
                    degree[in_state_num, 1]++;
                }
            }
            for (int i = 0; i < state_number; i++)
            {
                for (int j = 0; j < output.Count; j++)
                {
                    int idx = Convert.ToInt32(output[j].Substring(1));
                    if (degree[idx, 1] > degree[i, 1])
                    {
                        output.Insert(idx, "M" + i);
                        break;
                    }
                }
            }
            return output;
        }

        //静态度乘积法
        public static List<int> StaticTimesDegree(StateMachine m)
        {
            List<string> output = new List<string>();
            int state_number = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            int[] product = new int[state_number];
            int[,] degree = new int[state_number, 2];//0行为出度，1行为入度
            for (int i = 0; i < state_number; i++)
            {
                product[i] = 0;
                degree[i, 0] = 0;
                degree[i, 1] = 0;
            }
            foreach (State s in m.stateList)
            {
                int out_state_num = Convert.ToInt16(s.identifier.Substring(1));
                degree[out_state_num, 0] = s.transitions.Count;
                foreach (Transition t in s.transitions)
                {
                    int in_state_num = Convert.ToInt16(t.target.identifier.Substring(1));
                    degree[in_state_num, 1]++;
                }
            }
            for (int i = 0; i < state_number; i++) product[i] = degree[i, 0] * degree[i, 1];
            return product.ToList();
        }

        //静态度乘积法
        public static List<string> ZeroA(StateMachine m)
        {
            List<string> output = new List<string>();
            int state_number = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            int[] product = new int[state_number];
            int[,] degree = new int[state_number, 2];//0行为出度，1行为入度
            for (int i = 0; i < state_number; i++)
            {
                product[i] = 0;
                degree[i, 0] = 0;
                degree[i, 1] = 0;
            }
            foreach (State s in m.stateList)
            {
                int out_state_num = Convert.ToInt16(s.identifier.Substring(1));
                degree[out_state_num, 0] = s.transitions.Count;
                foreach (Transition t in s.transitions)
                {
                    int in_state_num = Convert.ToInt16(t.target.identifier.Substring(1));
                    degree[in_state_num, 1]++;
                }
            }
            for (int i = 0; i < state_number; i++) product[i] = degree[i, 0] * degree[i, 1];
            for (int i = 0; i < state_number; i++)
            {
                for (int j = 0; j < output.Count; j++)
                {
                    int idx = Convert.ToInt32(output[j].Substring(1));
                    if (product[idx] > product[i])
                    {
                        output.Insert(idx, "M" + i);
                        break;
                    }
                }
            }
            return output;
        }


        // 动态度乘积法
        public static List<List<string>> DynamicTimesDegree(StateMachine m)
        {
            //0. 初始化
            double shortest = 0;
            List<double> price = new List<double>();                              //存储值
            List<double[,]> matrixlist = new List<double[,]>();             //以队列结构存储转移矩阵
            List<List<string>> orderlist = new List<List<string>>();        //以队列结构存储对应转移矩阵的消减序列
            List<List<string>> shortest_orders = new List<List<string>>();

            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            double ancestor_price = 0;
            double[] in_degree = new double[count_States];
            double[] out_degree = new double[count_States];
            double[] loop = new double[count_States];

            double[,] ancestor = new double[count_States, count_States];
            for (int x = 0; x < count_States; x++) for (int y = 0; y < count_States; y++) ancestor[x, y] = 0;
            for (int x = 0; x < count_States; x++) { in_degree[x] = 0; out_degree[x] = 0; loop[x] = 0; }
            foreach (State s in m.stateList)
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

            //BFS搜索开始
            while (price.Count > 0)
            {

                double[,] Ri = (double[,])matrixlist[0].Clone();

                for (int z = 1; z < count_States - 1; z++)
                {
                    bool isvalue = false;
                    double current_price = 0, parent_count = 0;
                    List<string> current_order = orderlist[0].ToList();
                    double[,] Rj = new double[count_States, count_States];
                    for (int x = 0; x < count_States; x++)
                        if (Ri[x, z] != 0 || Ri[z, x] != 0) isvalue = true;
                    if (!isvalue) continue;

                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                        {
                            Rj[x, y] = Ri[x, y];
                            double Riz = Ri[x, z];
                            double Rzz = Ri[z, z];
                            double Rzy = Ri[z, y];
                            if (x == z || y == z) continue;
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
                            if (x == z || y == z) Rj[x, y] = 0;     //消减冗余信息

                    current_order.Add(z.ToString());

                    //计算权重值
                    int current_in_degree = 0, current_out_degree = 0;
                    for (int x = 0; x < count_States; x++)
                    {
                        if (x != z && Ri[x, z] > 0) current_in_degree++;
                        if (x != z && Ri[z, x] > 0) current_out_degree++;
                    }
                    current_price += current_in_degree * current_out_degree;

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

        public static List<List<string>> DynamicDM(StateMachine m)
        {
            //0. 初始化
            double shortest = 0;
            List<double> price = new List<double>();                              //存储值
            List<double[,]> matrixlist = new List<double[,]>();             //以队列结构存储转移矩阵
            List<List<string>> orderlist = new List<List<string>>();        //以队列结构存储对应转移矩阵的消减序列
            List<List<string>> shortest_orders = new List<List<string>>();

            int count_States = Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1;
            double ancestor_price = 0;
            double[] in_degree = new double[count_States];
            double[] out_degree = new double[count_States];
            double[] loop = new double[count_States];

            double[,] ancestor = new double[count_States, count_States];
            for (int x = 0; x < count_States; x++) for (int y = 0; y < count_States; y++) ancestor[x, y] = 0;
            for (int x = 0; x < count_States; x++) { in_degree[x] = 0; out_degree[x] = 0; loop[x] = 0; }
            foreach (State s in m.stateList)
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

            //BFS搜索开始
            while (price.Count > 0)
            {

                double[,] Ri = (double[,])matrixlist[0].Clone();

                for (int z = 1; z < count_States - 1; z++)
                {
                    bool isvalue = false;
                    double current_price = 0, parent_count = 0;
                    List<string> current_order = orderlist[0].ToList();
                    double[,] Rj = new double[count_States, count_States];
                    for (int x = 0; x < count_States; x++)
                        if (Ri[x, z] != 0 || Ri[z, x] != 0) isvalue = true;
                    if (!isvalue) continue;

                    for (int x = 0; x < count_States; x++)
                        for (int y = 0; y < count_States; y++)
                        {
                            Rj[x, y] = Ri[x, y];
                            double Riz = Ri[x, z];
                            double Rzz = Ri[z, z];
                            double Rzy = Ri[z, y];
                            if (x == z || y == z) continue;
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
                            if (x == z || y == z) Rj[x, y] = 0;     //消减冗余信息

                    current_order.Add(z.ToString());

                    //计算权重值
                    double current_weight = 0;
                    for (int x = 0; x < count_States; x++)
                    {
                        for (int y = 0; y < count_States; y++)
                            current_weight += Rj[x, y];
                    }
                    current_price += current_weight;

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
