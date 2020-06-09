using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//说明：状态机定义一组状态和状态间的转移关系。转移需要标明触发转移的事件。每个状态机有一个初始状态和多个终止状态。
namespace MZQStringRuleMining.Automaton
{
    public class StateMachine : IEquatable<StateMachine>
    {
        public string desciption;//状态语义说明

        public State start;//开始状态，一个自动机只有一个开始状态

        public int stateNum=0; //状态编号

        public List<State> stateList = new List<State>();//状态列表

        //构造函数
        public StateMachine()
        {
            start = new State("S0");

            start.type = "S";

            desciption = "状态机";

            stateList.Add(start);

        }

        public State getStartState()
        {
            return start;
        }
        //重写equal方法
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            StateMachine objAsStateMachine = obj as StateMachine;
            if (objAsStateMachine == null) return false;
            else return Equals(objAsStateMachine);
        }
        //接口函数
        public bool Equals(StateMachine statemachine)
        {
            bool flag = false;
            if (statemachine == null) return false;
            if (this.start.identifier.Equals(statemachine.start.identifier))
            {
                flag = true;
                int count = 0;
                for (int i = 0; i < this.stateList.Count; i++)
                    for (int j = 0; j < statemachine.stateList.Count; j++)
                    {
                        State this_state = stateList[i];
                        State target_state = statemachine.stateList[j];
                        if (this_state.Equals(target_state)) { count++; break; }
                    }
                if (count == this.stateList.Count) flag = true;
                else flag = false;
            }
            return flag;
        }
        public List<State> getEndState()
        {
            List<State> list = new List<State>();
            getDFSStates(start, ref list);

            List<State> ends = new List<State>();

            foreach (State s in list)
            {
                if (s.type == "E") ends.Add(s);
            }

            if (ends.Count == 0)
            {
                ends.Add(start);
            }
            return ends;
        }

        //显示状态机，生成一组状态-边标记-状态的集合，然后用R程序DisplayAutomaton.R可视化显示。  
        //带循环时有BUG
        public void display(State s, Stack<State> mark)
        {
            if (mark.Contains(s)) return;
            else mark.Push(s);
            foreach (Transition t in s.transitions)
            {
                Console.WriteLine(s.type+s.identifier.Substring(1)+"\t " +t.identifier+" \t"+t.target.type+t.target.identifier.Substring(1));
                display(t.target, mark);
            }
        }
        //克隆一个状态机
        public StateMachine clone()
        {
            StateMachine sm = new StateMachine();

            sm.desciption = desciption;
           
            sm.stateList.Clear();
            sm.start = start.clone(sm);            
            return sm;
        }
        public State searchState(State s, string name, ref List<State> list)
        {
            if (s.identifier.Equals(name)) return s;
            else 
            { 
                if (!list.Contains(s))
                {
                    list.Add(s);
                    foreach (Transition t in s.transitions)
                    {
                       State s_e= searchState(t.target, name,ref list);
                       if (s_e != null) return s_e; 
                    }
                   
                }          
               return null;
            }
        }
        //生成临接表，临接表索引是状态标号，临接表i个元素存储Si的孩子节点的标号。例如第j个元素中存储{5,6,12}，就代表状态Sj有三个孩子，分别是S5,S6和S12。
        //返回的临接表不包括初态和终态。
        public List<HashSet<int>> generateAdjacentList()
        {
            List<State> states = new List<State>();

            getDFSStates(start, ref states);

            int max = states.Max(delegate(State s) { return int.Parse(s.identifier.Substring(1)); });

            HashSet<int>[] adjacentList = new HashSet<int>[max + 1];

            foreach (State s in states)
            {
                if (s.type == "E" || s.type == "S") continue;

                int i = int.Parse(s.identifier.Substring(1));

                HashSet<int> set = new HashSet<int>();
                foreach (Transition t in s.transitions)
                {
                    set.Add(int.Parse(t.target.identifier.Substring(1)));
                }
                adjacentList[i] = set;
            }

            return new List<HashSet<int>>(adjacentList);
        }
        //按深度优先策略，获取自动机的状态列表，第一次调用时num应设为1。s为自动机的初态
        public void getDFSStates(State s, ref List<State> list)
        {
            bool isfind = false;
            foreach (State i in list)
                if (i.identifier.Equals(s.identifier))
                    isfind = true;
            if (!isfind)
            {
                list.Add(s);
                isfind = false;
                foreach (Transition t in s.transitions)
                {
                    getDFSStates(t.target,ref list);
                }
            }            
        }
        //获得自动机自终态逆向回溯前续状态至到初态的一个状态序列，结果存放在list中。在调用函数之前，应将终态事先放入list。
        //mark列表用于标识状态是否遍历过。
        public void getReverseStateList(State s, ref List<State> list)
        {
            while (!list.Contains(s))
            {
                List<State> mark = new List<State>();
                getPreStates2EndList(s, ref list, ref mark);
            }
        }
        public void getPreStates2EndList(State s, ref List<State> list, ref List<State> mark)
        {
            if (!mark.Contains(s))
            {
                mark.Add(s);
                foreach (Transition t in s.transitions)
                {
                    if (list.Contains(t.target)&& !list.Contains(s)) list.Add(s);
                    getPreStates2EndList(t.target, ref list, ref mark);
                }
            }            
        }
        //将状态机转换为正则表达式，这个方法还未完成。
        public string toRE()
        {
            string re = "";

            StateMachine sm = this.clone();

            Transition tt=new Transition("MZQ");

            tt.target=sm.start;

            Stack<State> s = new Stack<State>();

            display(start,s);

            Console.WriteLine("--------------------");

            sm.start.transitions[0].target.transitions[0].target.transitions.Add(tt);

            s.Clear();

            sm.display(sm.start,s);


            //Console.WriteLine("start of clone版本");

            //display(sm.start);

            //Console.WriteLine("end of clone版本");

            //规则1 检测处理环=> *
            while (true) 
            { 
                //(1) 检测环
                Stack<State> stack = new Stack<State>();

                List<State> cycle = detectCycle(sm.start,stack);

                List<State> cycle_bak = new List<State>(cycle.ToArray());

                if (cycle == null||cycle.Count==0)
                {
                    Console.WriteLine("on cycle");
                
                    break;
                }
                
                /*(2) 合并环cycle中包含的状态
                 * 创建一个新状态NewS，其identifier为cycle中标号最小的状态标识号；
                 * 将各状态的transitions合并到一起，赋于NewS；
                 * 将指向cycle中的transition的target指向NewS。
                 * 合并状态后会出现多个指向自已的transition，去掉重复transition，只保留一个。
                */
                //遍历自动机找到cycle中一个状态，第一个发现的元素也就是整个环的头。
                Stack<State> ss = new Stack<State>();

                bool isFind = false;

                ss.Push(sm.start);

                while (!isFind)
                {
                    State st = ss.Pop();
                
                    if (cycle.Contains(st))
                    {
                        isFind = true;
                        //开始合并状态
                        string label = "";

                        cycle.Remove(st);
                        while (cycle.Count > 0)
                        {
                            int i=st.transitions.Count-1;

                            while(i>=0)
                            {
                                Transition t = st.transitions[i];
                            
                                if(cycle.Contains(t.target)) 
                                {
                                    label += t.identifier;

                                    st.transitions.Remove(t);//去掉该转移，最后统一加指向自身的转移。

                                    st.transitions.AddRange(t.target.transitions);//将孩子的转移并入st的转移列表。
                                
                                    cycle.Remove(t.target);
                                }
                                i--;
                            }
                        }
                        //合并循环路径上的字符
                        foreach (Transition t in st.transitions)
                        {
                            if (t.target == st) {
                                t.identifier = label + t.identifier;
                                break;
                            }
                        }

                        List<State> c = new List<State>();
                    
                        convertTransition(sm.start, cycle_bak, st,c);

                        break;//结束状态合并
                    }
                    else
                    { //如果栈顶状态不是要找的状态，则查找该状态的孩子状态。
                        foreach (Transition t in st.transitions)
                            stack.Push(t.target);
                    }
                }
                Console.WriteLine("--------------------");
                s.Clear();

                sm.display(sm.start, s);

          }
            //规则2 检测处理顺序=> 普通字符序列

           

            //规则3 检测处理分支=> |


            Console.WriteLine("--------------------");
            s.Clear();

            sm.display(sm.start, s);

            return re;
        }
        //将以start为初始状态的自动机中指向状态列表xs中状态的转移全部修改为指向y,mark用于标识遍历过的状态。
        public void convertTransition(State start, List<State> xs, State y, List<State> mark)
        {
            mark.Add(start);
            foreach (Transition t in start.transitions)
            {
                if(!mark.Contains(t.target)) convertTransition(t.target, xs, y,mark);
           
                if (xs.Contains(t.target)) t.target = y;                
            }
            return;
        }
        //检测环，但不包括元素自身环。
        public List<State> detectCycle(State arg,Stack<State> stack)        
        {            
            if (stack.Contains(arg))//判断栈stack中是否存在状态arg，若存在，则证明有环，将栈中从arg元素到栈顶的元素返回。
            {
               //在栈转换成数组，找到arg位置，将从arg到栈顶的元素返回。
               State[] ss=stack.ToArray<State>();
               int i = ss.Length-1;
               while (ss[i] != arg&&i>=0) i--;
               List<State> r = new List<State>();
               while (i >=0)
               {
                   r.Add(ss[i]);
                   i--;
               }
               return r;
            }
            else 
            {
                stack.Push(arg);//将状态arg压入栈顶。
               
                for (int i = 0; i < arg.transitions.Count; i++)
                {
                    List<State> rr=new List<State>();

                    if(arg!=arg.transitions[i].target) rr=detectCycle(arg.transitions[i].target, stack);

                    if ( rr!= null) return rr; 
                }

                stack.Pop();//由于栈顶元素没孩子，所以弹出栈。

                return null;//说明此结点后面没有环
            
                                 
            }
            
        }

        //返回从初态到终状态的必经状态路径。返回的路径不包括初态和终态。
        //前提条件：有且只有一个初态和一个终态。
        public List<State> getDominatorSequence()
        { 
           List<State> Dominators=new List<State>();
           
           List<State> states=new List<State>();

           getDFSStates(start, ref states);
           int nums=0,/*初态数量*/nume=0/*终态数量*/;
           foreach (State ss in states)
           { 
              if(ss.type=="S") {nums++;}
              if(ss.type=="E") {nume++;}
           }
           //if(nums!=1) {
           //    Console.WriteLine("要得到必经结点序列，自动机有且只能有一个初态。该自动机初态数目为："+nums);
           //    return null;
           //}

           //if(nume!=1) {
           //    Console.WriteLine("要得到必经结点序列，自动机有且只能有一个终态。该自动机初态数目为："+nume);
           //    return null;
           //}

           foreach (State ss in states)
           {
               if (ss.type != "S" && ss.type != "E") 
               { 
                  //去除ss
                   StateMachine m_bak = this.clone();
                   State start_bak = m_bak.getStartState();
                   State end_bak = m_bak.getEndState()[0];

                   List<State> list = new List<State>();
                   m_bak.getDFSStates(m_bak.start,ref list);

                   foreach (State ss_bak in list)
                   { 
                      for(int i=ss_bak.transitions.Count-1;i>=0;i--)
                      {
                         Transition t_bak=ss_bak.transitions[i];
                         if (t_bak.target.identifier == ss.identifier) ss_bak.transitions.RemoveAt(i);
                      }                           
                   }

                  
                   List<State> v=new List<State>();
                   Stack<State> p=new Stack<State>();
                   //查看备份自动机m_bak的联通性。
                   List<State> path=m_bak.getDFSPath(start_bak, end_bak, ref v, ref p);
                   if (path == null) Dominators.Add(ss);
               }           
           }

           return Dominators;
        }
        //求状态机去除初态和终态后的不相交子状态机集合。
        
        public List<StateMachine> getDisjointSubMachines()
        {
            List<StateMachine> resultMachines = new List<StateMachine>();
            //判断状态机是否只有一个初态和终态。
            List<State> states=new List<State>();
            states.Clear();
            getDFSStates(start, ref states);
            int nums=0,/*初态数量*/nume=0/*终态数量*/;
           

           foreach (State ss in states)
           { 
              if(ss.type=="S") {nums++;}
              if(ss.type=="E") {nume++;}
           }

            if (nums != 1)
            {
                Console.WriteLine("要得到不相交的子状态机，自动机有且只能有一个初态。该自动机初态数目为：" + nums);
                return null;
            }

            if (nume != 1)
            {
                Console.WriteLine("要得到不相交的子状态机，自动机有且只能有一个终态。该自动机初态数目为：" + nume);
                return null;
            }
            //前提条件检验完毕。

            State end=getEndState()[0];

            List<Transition> transitions = new List<Transition>();
            foreach (Transition tr in start.transitions)
            {
                transitions.Add(tr);
            }

            if (transitions.Count < 2)
            { //返回去除初态和终态的子自动机。
                if (transitions.Count < 1) return null;//自动机只有一个初态，无子自动机。
                else
                {
                    resultMachines.Add(getSubMachine(start, end));
                }
            }
            else 
            {
                List<List<Transition>> transGroups = new List<List<Transition>>();//每个元素是转移的集合。

                while (transitions.Count>0)
                {
                    List<Transition> transGroup = new List<Transition>();
                    transGroup.Add(transitions[transitions.Count-1]);
                    
                    List<State> la = new List<State>();
                    getDFSStates(transitions[transitions.Count - 1].target, ref la);
                    
                    HashSet<Transition> ha = new HashSet<Transition>();//获得该组转移所指向状态连通的转移集合。
                    //首先把从初态指向该子自动机的转移加入。

                    ha.Add(transitions[transitions.Count - 1]);

                    transitions.RemoveAt(transitions.Count - 1);


                    foreach (State s in la)
                    {
                        foreach (Transition t in s.transitions)
                        {
                            ha.Add(t);
                        }
                    }
                    
                    for (int i = transitions.Count-1; i >=0 ; i--)
                    {                       
                       List<State> lb = new List<State>();
                       getDFSStates(transitions[i].target, ref lb);
                       HashSet<Transition> hb = new HashSet<Transition>();

                       foreach (State s in lb)
                       {
                           foreach (Transition t in s.transitions)
                           {
                               hb.Add(t);
                           }
                       }
                       //最后把从初态指向该子自动机的转移加入。
                       hb.Add(transitions[i]);

                       hb.IntersectWith(ha);

                       if (hb.Count > 0)//若存在相同转移则子自动机a与b相交。
                       {
                           transGroup.Add(transitions[i]);
                           transitions.RemoveAt(i);
                       }
                    }
                    transGroups.Add(transGroup);
                }    
                //根据tranBag构造子自动机。
                foreach (List<Transition> ts in transGroups)
                {
                    StateMachine sm = this.clone();
                    for(int i= sm.start.transitions.Count-1;i>=0;i--)
                    {                       
                        bool isFind = false;
                        foreach (Transition t in ts)
                        {
                            if (t.identifier.Equals(sm.start.transitions[i].identifier) && t.target.identifier.Equals(sm.start.transitions[i].target.identifier))
                            {
                                isFind = true;
                                break;
                            }
                        }
                        if(!isFind) sm.start.transitions.RemoveAt(i);
                    }
                    resultMachines.Add(sm);
                }
            }
            return resultMachines;
        }
        //给定两个状态a,b。从a开始生成子自动机，并剪掉b开始的子自动机。
        //给定两个状态a,b。从a开始生成子自动机，并剪掉b开始的子自动机。
        public StateMachine getSubMachine(State a, State b)
        {
            //List<State> dfsStates = new List<State>();
            //getDFSStates(a, ref dfsStates);无意义？——祖祈注

            StateMachine subMachine = new StateMachine();
            subMachine.stateList.Clear();//防止双初态——祖祈注
            subMachine.start = a.clone(subMachine);
            subMachine.start.type = "S";

            List<State> SL = new List<State>();
            subMachine.getDFSStates(subMachine.start, ref SL);

            foreach (State s in SL)//剪掉除指向自己外的所有子自动机——祖祈注
            {
                if (s.identifier.Equals(b.identifier))
                {
                    for (int i = s.transitions.Count - 1; i >= 0; i--)
                        if (!s.transitions[i].target.identifier.Equals(b.identifier))
                            s.transitions.RemoveAt(i);
                    s.type = "E";
                }
            }

            return subMachine;
        }
        
        public void setSingleEndState()
        {
            List<State> order = new List<State>();

            getDFSStates(start, ref order);
                  

            State end = new State("S"+stateNum++);
            end.type = "E";

            for (int i = order.Count - 1; i >= 0; i--)
            {
                if (order[i].type == "E")
                {
                    order[i].type = "M";
                    Transition newTr = new Transition("");
                    newTr.target = end;
                    order[i].transitions.Add(newTr);
                }//order.RemoveAt(i);
            }         
        }

        //s是起始状态,e是目标状态，visited是记录遍历过的状态，stack记录路径
        //返回的List是一个从目标状态到起始状态的倒序排列的路径。
        public List<State> getDFSPath(State s, State e, ref List<State> visited, ref Stack<State> stack)
        {
            stack.Push(s);
            if (s == e)
            {
                return new List<State>(stack.ToArray());
            }
            else
            {
                if (!visited.Contains(s))
                {
                    visited.Add(s);
                    foreach (Transition t in s.transitions)
                    {
                        List<State> temp = getDFSPath(t.target, e, ref visited, ref stack);
                       if (temp != null) return temp;
                    }
                }
                stack.Pop();
                return null;
            }
            
        }

        /* 功能说明：向状态机添加一个状态的所有事件序列（用字符标识事件）
      *  输入参数：文本文件中的一行字符串
      *  返回参数：无
      *  备    注：button4调用
      */
        public void InsertAutomata(List<string> InputStrings)
        {

            State current = start;
            State s = start;

            foreach (string arg in InputStrings)
            {
                bool isFind_first = false, isFind_tail = false;
                int i = 0;
                string first_node = null;
                string event_node = null;
                string tail_node = null;

                if (arg.Equals("")) continue;

                for (; arg[i] != '\t'; i++) first_node += arg[i];
                for (i = i + 1; arg[i] != '\t'; i++) event_node += arg[i];
                for (i = i + 1; i < arg.Length; i++) tail_node += arg[i];

                foreach (State state in stateList)
                {
                    if (state.identifier.Equals(first_node))
                    {
                        isFind_first = true;
                        current = state;
                        break;
                    }
                }

                if (!isFind_first)
                {
                    current = new State(first_node);
                    stateList.Add(current);
                }

                Transition newTrans = new Transition(event_node);

                foreach (State state in stateList)
                {
                    if (state.identifier.Equals(tail_node))
                    {
                        s = state;
                        isFind_tail = true;
                        break;
                    }
                }

                if (!isFind_tail)
                {
                    s = new State(tail_node);
                    stateList.Add(s);
                }

                if (tail_node[0] == 'E') s.type = "E";
                newTrans.target = s;
                current.transitions.Add(newTrans);
                current = newTrans.target;
            }

            for (int k = 0; k < stateList.Count; k++)
            {
                for (int i = 0; i < stateList[k].transitions.Count; i++)
                {
                    for (int j = i + 1; j < stateList[k].transitions.Count; j++)
                    {
                        string contr_state = stateList[k].transitions[i].target.identifier;
                        string exper_state = stateList[k].transitions[j].target.identifier;
                        if (contr_state.Equals(exper_state))
                        {
                            if (!stateList[k].transitions[i].identifier.Equals(stateList[k].transitions[j].identifier))
                                stateList[k].transitions[i].identifier += ("|" + stateList[k].transitions[j].identifier);
                            stateList[k].transitions.RemoveAt(j--);
                        }
                    }
                }
            }
        }


        public StateMachine getSubGraph(State a, State b, List<State> forwardState, List<State> backwardState)
        {
            //List<State> dfsStates = new List<State>();
            //getDFSStates(a, ref dfsStates);无意义？——祖祈注

            StateMachine subMachine = new StateMachine();
            subMachine.stateList.Clear();//防止双初态——祖祈注
            subMachine.start = a.clone(subMachine);
            subMachine.start.type = "S";

            List<State> SL = new List<State>();
            subMachine.getDFSStates(subMachine.start, ref SL);

            foreach (State s in SL)
            {
                if (s.identifier.Equals(a.identifier))      //保留从始态出发的前向边，删除从始态出发的后向边
                {
                    for (int i = s.transitions.Count - 1; i >= 0; i--)
                        foreach (State s_temp in forwardState)
                            if (s_temp.identifier.Equals(s.transitions[i].target.identifier))
                            { s.transitions.RemoveAt(i); break; }
                }
                else if (s.identifier.Equals(b.identifier))  //保留从终态出发的后向边，删除从终态出发的前向边
                {
                    for (int i = s.transitions.Count - 1; i >= 0; i--)
                    {
                        bool flag = false;
                        foreach (State s_temp in backwardState)
                            if (s_temp.identifier.Equals(s.transitions[i].target.identifier))
                                flag = true;
                        if (!flag) s.transitions.RemoveAt(i);
                    }
                    s.type = "E";
                }
            }

            return subMachine;
        }

        public void GenerateEFAbyMatrix(double[,] adj_matrix)
        {
            State current = start;
            State s = start;

            for (int i = 0; i < adj_matrix.GetLength(0); i++)
                for (int j = 0; j < adj_matrix.GetLength(1); j++)
                {
                    bool isFind_first = false, isFind_tail = false;
                    string first_node = (i == 0 ? "S0" : ("M" + i));
                    string event_node = adj_matrix[i, j].ToString();
                    string tail_node = (j == adj_matrix.GetLength(1) - 1 ? ("E" + j) : ("M" + j));

                    if (adj_matrix[i, j] == 0) continue;

                    foreach (State state in stateList)
                    {
                        if (state.identifier.Equals(first_node))
                        {
                            isFind_first = true;
                            current = state;
                            break;
                        }
                    }

                    if (!isFind_first)
                    {
                        current = new State(first_node);
                        stateList.Add(current);
                    }

                    Transition newTrans = new Transition(event_node);

                    foreach (State state in stateList)
                    {
                        if (state.identifier.Equals(tail_node))
                        {
                            s = state;
                            isFind_tail = true;
                            break;
                        }
                    }

                    if (!isFind_tail)
                    {
                        s = new State(tail_node);
                        stateList.Add(s);
                    }

                    if (tail_node[0] == 'E') s.type = "E";
                    newTrans.target = s;
                    current.transitions.Add(newTrans);
                    current = newTrans.target;
                }
        }

        public static StateMachine Matrix2Machine(double[,] matrix)
        {
            List<string> machine_log = new List<string>();
            StateMachine machine = new StateMachine();
            int s_count = matrix.GetLength(0);

            for (int x = 0; x < s_count; x++)
                for (int y = 0; y < s_count; y++)
                    if (matrix[x, y] > 0)
                    {
                        if (x == 0 && y == (s_count - 1))
                            machine_log.Add(string.Format("S0\t{0}\tE{1}", matrix[x, y], y));
                        else if (x == 0 && y != (s_count - 1))
                            machine_log.Add(string.Format("S0\t{0}\tM{1}", matrix[x, y], y));
                        else if (x != 0 && y == (s_count - 1))
                            machine_log.Add(string.Format("M{0}\t{1}\tE{2}", x, matrix[x, y], y));
                        else
                            machine_log.Add(string.Format("M{0}\t{1}\tM{2}", x, matrix[x, y], y));
                    }
            machine.InsertAutomata(machine_log);
            return machine;
        }

        public void RemoveState(string id)
        {
            for (int s_idx = stateList.Count - 1; s_idx >= 0; s_idx-- )
            {
                if (stateList[s_idx].identifier == id)
                {
                    stateList.RemoveAt(s_idx);
                    continue;
                }
                for (int t_idx = stateList[s_idx].transitions.Count - 1; t_idx >= 0; t_idx--)
                {
                    if (stateList[s_idx].transitions[t_idx].target.identifier == id)
                    {
                        stateList[s_idx].transitions.RemoveAt(t_idx);
                        continue;
                    }
                }
            }
        }

        public void AddEdge(string start, string end, string c)
        {
            bool s_no = false, e_no = false;
            for (int idx = 0; idx < stateList.Count; idx++)
            {
                if (stateList[idx].identifier == start) s_no = true;
                if (stateList[idx].identifier == end) e_no = true;
                if (s_no && e_no) break;
            }
            if (!s_no)
            {
                State s = new State(start);
                stateList.Add(s);
            }
            if (!e_no)
            {
                State s = new State(end);
                stateList.Add(s);
            }
            Transition t = new Transition(c);
            for (int i = stateList.Count - 1; i >= 0 ; i--)
            {
                if (stateList[i].identifier == start)
                {
                    for (int j = stateList.Count - 1; j >= 0; j--)
                    {
                        if (stateList[j].identifier == end)
                        {
                            t.target = stateList[j];
                            stateList[i].transitions.Add(t);
                            return;
                        }
                    }
                }
            }
            
        }

    }
}
