using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class NeccessaryPath
    {
        //获得状态消减序列：基于必经路径。参考论文Yo-Sub Han, Derick Wood.Obtaining shorter regular expressions from finite-state automata.2007
        //（1）识别必经路径(dominated tree)
        //（2）给定识别路径上两个状态，抽取子自动机
        //（3）识别除了初态与终态外状态集不相交的子自动机的集合。
        /* 给定一个自动机A和一个空栈STACK。
         * 执行（1）=>必经路径。
         * 若必经路径不为空，则
         *    将必经路径上的状态压入栈 STACK。
         *    在必经路径上依次选择紧邻的两个状态s和f（在必经路径上不能存在任何间间状态q,使q在s后面，又在f前面出现），执行（2）识别以s为初态,f为终态的子自动机Ai，
         *    若Ai存在中间状态，则将Ai的状态按消减序列压入栈STACK，否则直接返回。
         * 若必经路径为空，则
         *    执行（3）=>识别不相交的子自动机的集合Bi。将Bi状态按消减序列压入栈STACK。
         * 
         */
        public static List<string> getStateEliminationSequenceBasedonNecessaryPath(StateMachine m, ref Stack<State> STACK, ref List<State> nec_Path)
        {
            StateMachine machine = new StateMachine();
            machine = m.clone();
            List<State> dfsStates = new List<State>();
            int cycle = machine.stateList.Count;
            //判断自动机是否存在冗余状态
            while (cycle-- > 0)
            {
                Hashtable weight_in = new Hashtable();
                Hashtable weight_out = new Hashtable();
                //初始化状态与权重的键值对哈稀表。
                foreach (State s in machine.stateList)
                {
                    if (s.type != "S" && s.type != "E")
                    {
                        weight_in.Add(s, 0);
                        weight_out.Add(s, 0);
                    }
                }
                foreach (State s in machine.stateList)
                {
                    foreach (Transition t in s.transitions)
                    {
                        if (t.target != s && t.target.type != "S" && t.target.type != "E") weight_in[t.target] = (int)weight_in[t.target] + 1;
                        if (t.target != s && s.type != "S" && s.type != "E") weight_out[s] = (int)weight_out[s] + 1;
                    }
                }
                for (int x = 0; x < machine.stateList.Count; x++)
                {
                    if (machine.stateList[x].type.Equals("S") || machine.stateList[x].type.Equals("E")) continue;
                    if ((int)weight_in[machine.stateList[x]] == 0 || (int)weight_out[machine.stateList[x]] == 0)
                    {
                        foreach (State s in machine.stateList)
                        {
                            for (int y = 0; y < s.transitions.Count; y++)
                                if (s.transitions[y].target.identifier.Equals(machine.stateList[x].identifier))
                                    s.transitions.RemoveAt(y--);
                        }
                        machine.stateList.RemoveAt(x--);
                    }
                }
            }
            //原代码
            List<State> necessaryPath = machine.getDominatorSequence();//必经路径
            nec_Path = necessaryPath.ToList();
            machine.getDFSStates(machine.start, ref dfsStates);
            int count_States = dfsStates.Count;
            if (!machine.getEndState()[0].identifier.Substring(1).Equals(Convert.ToString(count_States - 1)))
                count_States = Convert.ToInt16(machine.getEndState()[0].identifier.Substring(1)) + 1;

            int[,] submatrix = new int[count_States, count_States];

            necessaryPath.Insert(0, machine.start);
            List<State> ends = machine.getEndState();
            State end = null;

            if (ends.Count > 0)
            {
                end = ends[0];
                necessaryPath.Add(end);
            }

            int i = 1;
            for (; i < necessaryPath.Count - 1; i++)
            {
                STACK.Push(necessaryPath[i]);
                StateMachine sm = machine.getSubMachine(necessaryPath[i - 1], necessaryPath[i]);
                List<StateMachine> vsm = sm.getDisjointSubMachines();
                if (vsm.Count == 1)
                {//无法再切分了。
                    StateMachine fvm = vsm[0];
                    //   List<State> lsequence = getStateEliminationSequence(fvm);

                    List<int> se = new List<int>();
                    se.Clear();
                    getStateEliminationSequenceBasedonAdjacentList(fvm.generateAdjacentList(), ref se);

                    //掐头去尾
                    //se.RemoveAt(0);
                    //se.RemoveAt(se.Count - 1);

                    List<State> lsequence = new List<State>();
                    for (int k = 0; k < se.Count; k++)
                    {
                        List<State> temps = new List<State>();
                        temps.Clear();
                        lsequence.Add(fvm.searchState(fvm.start, "M" + se[k], ref temps));
                    } //掐头去尾后不应存在始态——祖祈注
                    for (int j = lsequence.Count - 1; j >= 0; j--)
                    {
                        bool flag = false;
                        if (necessaryPath.Contains(lsequence[j])) continue;
                        foreach (State s in STACK) if (s.identifier.Equals(lsequence[j].identifier)) flag = true;
                        if (!flag) STACK.Push(lsequence[j]);
                    }
                }
                else
                {
                    foreach (StateMachine vm in vsm)
                    {
                        List<State> nec = new List<State>();
                        getStateEliminationSequenceBasedonNecessaryPath(vm, ref STACK, ref nec);
                    }
                }
            }
            StateMachine sm1 = machine.getSubMachine(necessaryPath[i - 1], necessaryPath[i]);
            List<StateMachine> vsm1 = sm1.getDisjointSubMachines();

            if (vsm1.Count == 1)
            {//无法再切分了。
                StateMachine fvm = vsm1[0];
                //   List<State> lsequence = getStateEliminationSequence(fvm);

                List<int> se = new List<int>();
                se.Clear();
                getStateEliminationSequenceBasedonAdjacentList(fvm.generateAdjacentList(), ref se);

                //掐头去尾
                //se.RemoveAt(0);
                //se.RemoveAt(se.Count - 1);

                List<State> lsequence = new List<State>();
                for (int k = 0; k < se.Count; k++)
                {
                    List<State> temps = new List<State>();
                    temps.Clear();
                    lsequence.Add(fvm.searchState(fvm.start, "M" + se[k], ref temps));
                }//掐头去尾后不应存在始态——祖祈注
                for (int j = lsequence.Count - 1; j >= 0; j--)
                {
                    bool flag = false;
                    if (necessaryPath.Contains(lsequence[j])) continue;
                    foreach (State s in STACK) if (s.identifier.Equals(lsequence[j].identifier)) flag = true;
                    if (!flag) STACK.Push(lsequence[j]);
                }
            }
            else
            {
                foreach (StateMachine vm1 in vsm1)
                {
                    List<State> nec = new List<State>();
                    getStateEliminationSequenceBasedonNecessaryPath(vm1, ref STACK, ref nec);
                }
            }

            List<State> last2 = new List<State>(STACK.ToArray());
            List<string> labels2 = new List<string>();

            for (int ii = 0; ii < last2.Count; ii++) labels2.Add(last2[ii].identifier);
            foreach (State s in m.stateList)
            {
                if (s.type.Equals("S") || s.type.Equals("E")) continue;
                bool empty = true;
                foreach (string str in labels2) if (str.Equals(s.identifier)) empty = false;
                if (empty) labels2.Add(s.identifier);
            }
            labels2.Add("S0");
            labels2.Add(m.getEndState()[0].identifier);
            return labels2;
        }

        public static void getStateEliminationSequenceBasedonAdjacentList(List<HashSet<int>> adjacentList, ref List<int> sequence)
        {
            if (adjacentList.Sum(delegate (HashSet<int> set1) { return set1 != null ? 1 : 0; }) == 0) return;
            //找孩子最少的状态
            int minCount = int.MaxValue;
            int minstate = 0;
            for (int i = 0; i < adjacentList.Count; i++)
            {
                if (adjacentList[i] == null) continue;

                if (adjacentList[i].Count < minCount)
                {
                    minCount = adjacentList[i].Count;
                    minstate = i;
                }
            }
            //假设去掉该状态
            HashSet<int> set = adjacentList[minstate];
            adjacentList[minstate] = null;
            sequence.Add(minstate);
            foreach (HashSet<int> s in adjacentList)
            {
                if (s != null && s.Contains(minstate))
                {
                    s.Remove(minstate);
                    s.UnionWith(set);
                }
            }
            getStateEliminationSequenceBasedonAdjacentList(adjacentList, ref sequence);
        }
    }
}
