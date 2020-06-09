using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class ThirdMethod
    {
        public static List<int> ThirdTechnique(StateMachine m)
        {

            List<string> order = new List<string>();
            List<State> necessaryPath = new List<State>(m.getDominatorSequence());
            List<State> bridge = new List<State>();
            List<State> end = new List<State>(m.getEndState());
            List<DirectNec> dir = new List<DirectNec>();        //记录前后必经结点
            List<StateMachine> sm = new List<StateMachine>();   //记录子图
            List<List<string>> cycleset = new List<List<string>>(ForthMethod.FloydCycle(m.clone()));  //BFS算法生成回路集
            CycleSet(m.clone(), necessaryPath, ref sm);   //DFS算法生成子图，原来用于生成回路集
            int l_state = Convert.ToInt16(end[0].identifier.Substring(1)) + 1;
            int[] weight_temp = new int[l_state];
            int[] weight_dyn = new int[weight_temp.Length];
            int[] weight = new int[weight_temp.Length];         //用于存储根据回路集所求的权重值

            //回路集计算权值，而后以递增顺序生成消减序列
            //权重计算方法——根据状态的出现次数计算权重值
            //举例：若某结点在回路集中出现n次，则该结点的权值为n
            foreach (List<string> cycle in cycleset)
                foreach (string state in cycle)
                {
                    int state_number = Convert.ToInt16(state.Substring(1));
                    weight_temp[state_number]++;
                }
            weight = weight_temp.ToArray();
            for (int i = 1; i < weight_dyn.Length - 1; i++)
            {
                int weight_number = 1;
                weight_dyn[i] = weight_temp[1];
                for (int j = 1; j < weight_temp.Length - 1; j++)
                    if (weight_dyn[i] > weight_temp[j])
                    {
                        weight_dyn[i] = weight_temp[j];
                        weight_number = j;
                    }
                weight_temp[weight_number] = 65535;
                order.Add("M" + weight_number);
            }
            return weight.ToList();
            
        }

        //深度搜索，查找指向某结点的路径终点为哪一个必经结点
        public static void NeccessaryStateSearch(State state, List<State> nec_path, ref List<State> nec_state, ref List<State> stack)
        {
            bool flag = false;
            foreach (State s in nec_path) if (s.identifier.Equals(state.identifier)) { flag = true; break; }
            if (flag) nec_state.Add(state);    //指向为必经结点，添加至列表
            else
                foreach (Transition t in state.transitions)             //指向为非必经结点
                {
                    if (t.target.Equals(state) || stack.Contains(t.target)) continue;              //排除自身环及已访问结点
                    else
                    {
                        stack.Add(t.target);
                        NeccessaryStateSearch(t.target, nec_path, ref nec_state, ref stack);
                        stack.Remove(t.target);
                    }
                }
        }

        //回路集合算法
        public static void CycleSet(StateMachine m, List<State> necessaryPath, ref List<StateMachine> sm)
        {
            //初始化变量
            List<List<string>> cs = new List<List<string>>();
            List<State> subgraph = new List<State>();
            List<State> forwardState = new List<State>();
            List<State> backwardState = new List<State>();
            List<State> end = new List<State>(m.getEndState());
            int l_state = Convert.ToInt16(end[0].identifier.Substring(1)) + 1;
            int[] C = new int[l_state];

            cs.Add(new List<string>());
            necessaryPath.Insert(0, m.start);
            necessaryPath.Add(end[0]);
            for (int i = 0; i < l_state; i++) C[i] = 0;
            //函数主体
            for (int i = 1; i < necessaryPath.Count; i++)
            {   //根据子图序列查找回路集合
                int end_number = 0;
                StateMachine submachine = new StateMachine();
                //以子图终态的转移关系为单位，判断子图终态的转移关系是否为后向边
                foreach (Transition t in necessaryPath[i].transitions)
                {
                    int count = 0;
                    List<State> nec_state = new List<State>();
                    List<State> stack = new List<State>();
                    NeccessaryStateSearch(t.target, necessaryPath, ref nec_state, ref stack);
                    foreach (State s in nec_state)
                    {
                        int number = 0;
                        foreach (State nec in necessaryPath)
                            if (nec.identifier.Equals(s.identifier))
                                number = necessaryPath.IndexOf(nec);
                        if (number <= i) count++;  //若搜索到终态前的必经结点，计数
                    }
                    if (count == nec_state.Count) backwardState.Add(t.target);
                    //若经过该转移关系的路径所到达的必经结点均在该子图终态之前，该转移关系为后向边；否则为前向边
                }
                submachine = m.getSubGraph(necessaryPath[i - 1], necessaryPath[i], forwardState, backwardState);
                submachine.getDFSStates(submachine.start, ref subgraph);    //以相邻的两个必经结点为界，分割子图
                for (int j = 0; j < subgraph.Count; j++)
                {   //若子图中包含之前子图的已处理结点，删除已处理结点
                    int state_number = Convert.ToInt16(subgraph[j].identifier.Substring(1));
                    if (C[state_number] == 1) subgraph.RemoveAt(j--);
                }

                sm.Add(submachine.clone());
                sm[sm.Count - 1].stateList = subgraph.ToList();
                C[end_number] = 0;  //本子图的终态为下一个子图的初态，故该结点状态C=0
                forwardState = backwardState.ToList();//本子图终态的后向边需要在下一个子图的初态中删除
                subgraph.Clear();
                backwardState.Clear();
            }
        }
        //打印消减过程中的转移矩阵
        

    }
}
