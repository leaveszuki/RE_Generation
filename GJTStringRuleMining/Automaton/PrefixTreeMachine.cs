using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//说明：前缀树自动机，前缀相同的字符序列合并成一条路径
namespace MZQStringRuleMining.Automaton
{
    class PrefixTreeMachine:StateMachine
    {     
         
        public PrefixTreeMachine()
        {
          
        }
        //向状态机添加一条字符序列或事件序列（用字符标识事件）
        public void addSequence(string arg)
        {
            State current = start;            
            for (int i = 0; i < arg.Length; i++)
            {
                bool isFind = false; 
                foreach (Transition t in current.transitions)   //寻找目标节点
                {
                    if (t.identifier.Equals(arg.Substring(i, 1)))
                    {
                        current = t.target;
                        if (i == arg.Length - 1) current.type = "E";
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {                     
                   Transition newTrans=new Transition(arg.Substring(i, 1));
                   State s= new State("S" + stateNum++);
                   newTrans.target = s;
                   stateList.Add(s);
                   if (i == arg.Length - 1) newTrans.target.type = "E";
                   current.transitions.Add(newTrans);
                   current = newTrans.target;
                }
            }
        }

        /*
           k尾文法 版本2 合并状态后不跳出循环迭代地查看是否存在等价关系，执行一遍2重循环后结束。
         * 2017-05-20
        */
        public void mergeKTails2(int K)
        {
           // bool existKTailEquivalentStates = false;
            List<State> stateList = new List<State>();
           
                stateList.Clear();
                getDFSStates(start, ref stateList);

                if (stateList == null)
                {
                    Console.WriteLine("空自动机");
                    return;
                }

               // List<int> garbageCase = new List<int>();//状态垃圾箱，存放合并后被丢弃的状态序号；比较过程中不比较垃圾箱中的状态。
                int[] garbageCase = new int[stateList.Count];
                
                for (int i = 0; i < garbageCase.Length; i++) garbageCase[i] = 0;//0代表该状态未丢弃，1代表该状态被丢弃，初始值都为0.

                for (int i = 0; i < stateList.Count; i++)
                {
                  // if (garbageCase.Contains(i)) continue;
                   if (garbageCase[i] == 1) continue;

                    if (stateList[i].type == "E") continue;//终态不参与合并
                    for (int j = i + 1; j < stateList.Count; j++)
                    {
                    //    if (garbageCase.Contains(j)) continue;
                        if (garbageCase[j] == 1) continue;

                        if (stateList[j].type == "E") continue;//终态不参与合并
                        //比较二者的后缀集是否相同。
                        Stack<State> ssA = new Stack<State>();
                        ssA.Clear();
                        ssA.Push(stateList[i]);
                        Stack<string> seA = new Stack<string>();
                        seA.Clear();
                        seA.Push("");
                        List<String> suffixA = new List<string>();
                        suffixA.Clear();
                        getSuffixes(ref ssA, ref seA, ref suffixA);

                        Stack<State> ssB = new Stack<State>();
                        ssB.Clear();
                        ssB.Push(stateList[j]);
                        Stack<string> seB = new Stack<string>();
                        seB.Clear();
                        seB.Push("");
                        List<String> suffixB = new List<string>();
                        suffixB.Clear();
                        getSuffixes(ref ssB, ref seB, ref suffixB);
                        //筛选长度小于k的后缀集合。
                        List<string> KsuffixA = suffixA.FindAll(delegate(string str) { return str.Length > K ? false : true; });
                        List<string> KsuffixB = suffixB.FindAll(delegate(string str) { return str.Length > K ? false : true; });

                        //若相同，标记existNerodEquivalentStates为true并合并两者。

                        //if (KsuffixA.All(KsuffixB.Contains) && (KsuffixA.Count == KsuffixB.Count) && (KsuffixB.Count!=0))
                        if (KsuffixA.All(KsuffixB.Contains) && (KsuffixA.Count == KsuffixB.Count))
                        {
                            merge(stateList[i], stateList[j]);

                       //       garbageCase.Add(j);
                            
                           garbageCase[j] = 1;

                            Console.WriteLine(System.DateTime.Now + " Merge States：" + stateList[i].identifier + " " + stateList[j].identifier);
                            //Stack<State> l = new Stack<State>();
                            //display(start, l);
                            //Console.WriteLine("原始自动机------------------------");                     
                        }
                    }
                }
          
        }

        //k尾文法
        public void mergeKTails(int K)
        {
            bool existKTailEquivalentStates = false;
            List<State> stateList = new List<State>();
            do
            {
                existKTailEquivalentStates = false;

                stateList.Clear();
                getDFSStates(start, ref stateList);

                if (stateList == null)
                {
                    Console.WriteLine("空自动机");
                    return;
                }
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (stateList[i].type == "E") continue;//终态不参与合并
                    for (int j = i + 1; j < stateList.Count; j++)
                    {
                        if (stateList[j].type == "E") continue;//终态不参与合并
                        //比较二者的后缀集是否相同。
                        Stack<State> ssA = new Stack<State>();
                        ssA.Clear();
                        ssA.Push(stateList[i]);
                        Stack<string> seA = new Stack<string>();
                        seA.Clear();
                        seA.Push("");
                        List<String> suffixA = new List<string>();
                        suffixA.Clear();
                        getSuffixes(ref ssA, ref seA, ref suffixA);

                        Stack<State> ssB = new Stack<State>();
                        ssB.Clear();
                        ssB.Push(stateList[j]);
                        Stack<string> seB = new Stack<string>();
                        seB.Clear();
                        seB.Push("");
                        List<String> suffixB = new List<string>();
                        suffixB.Clear();
                        getSuffixes(ref ssB, ref seB, ref suffixB);
                        //筛选长度小于k的后缀集合。
                        List<string> KsuffixA = suffixA.FindAll(delegate(string str) { return str.Length > K ? false : true; });
                        List<string> KsuffixB = suffixB.FindAll(delegate(string str) { return str.Length > K ? false : true; });

                        //若相同，标记existNerodEquivalentStates为true并合并两者。

                        //if (KsuffixA.All(KsuffixB.Contains) && (KsuffixA.Count == KsuffixB.Count) && (KsuffixB.Count!=0))
                        if (KsuffixA.All(KsuffixB.Contains) && (KsuffixA.Count == KsuffixB.Count))
                        {
                            existKTailEquivalentStates = true;
                            
                            merge(stateList[i], stateList[j]);
                            
                            Console.WriteLine(System.DateTime.Now  + " 合并状态："+stateList[i].identifier+" "+stateList[j].identifier);
                            //Stack<State> l = new Stack<State>();
                            //display(start, l);
                            //Console.WriteLine("原始自动机------------------------");
                            i = stateList.Count;
                            break;
                            //跳出两重循环。                            
                        }
                    }
                }
            } while (existKTailEquivalentStates);       
        }
        //规范微商文法为基础的推断算法，也就是基于尼罗德等价关系进行状态合并。
        public void mergeNerodeEquivalentStates()
        { 
          bool existNerodEquivalentStates=false;
          List<State> stateList = new List<State>();
          do
          {
              existNerodEquivalentStates = false;

              stateList.Clear();
              getDFSStates(start, ref stateList);

              if (stateList == null)
              {
                  Console.WriteLine("空自动机");
                  return;
              }
              for (int i = 0; i < stateList.Count; i++)
                  for (int j = i+1; j < stateList.Count; j++)
                  {
                      //比较二者的后缀集是否相同。
                      Stack<State> ssA = new Stack<State>();
                      ssA.Clear();
                      ssA.Push(stateList[i]);
                      Stack<string> seA = new Stack<string>();
                      seA.Clear();
                      seA.Push("");
                      List<String> suffixA = new List<string>();
                      suffixA.Clear();
                      getSuffixes(ref ssA, ref seA, ref suffixA);

                      Stack<State> ssB = new Stack<State>();
                      ssB.Clear();
                      ssB.Push(stateList[j]);
                      Stack<string> seB = new Stack<string>();
                      seB.Clear();
                      seB.Push("");
                      List<String> suffixB = new List<string>();
                      suffixB.Clear();
                      getSuffixes(ref ssB, ref seB, ref suffixB);

                      //若相同，标记existNerodEquivalentStates为true并合并两者。

                      if (suffixA.All(suffixB.Contains) && (suffixA.Count == suffixB.Count))
                      {
                          existNerodEquivalentStates = true;
                          //Console.WriteLine("自动机:合并状态前" + stateList[i].identifier + "," + stateList[j].identifier);
                          //Stack<State> ll = new Stack<State>();
                          //display(start, ll);

                          merge(stateList[i], stateList[j]);

                          //Console.WriteLine("自动机:合并状态" + stateList[i].identifier + "," + stateList[j].identifier);
                          //Stack<State> l = new Stack<State>();
                          //display(start, l);
                      }
                  }
          } while (existNerodEquivalentStates);       
        }
        //获得状态的后缀集合
        //ss在传递给getSuffixes之前，要将状态压入栈。
        //ss:当前路径上状态序列
        //se：当前路径上字母序列
        //result:后缀集合
        public void getSuffixes(ref Stack<State> ss, ref Stack<string> se, ref List<string> result)
        {
            State s = ss.Peek();
            foreach (Transition t in s.transitions)
            {
                if (ss.Contains(t.target)) continue;//如果转移是循环转移，则不把它加入后缀序列。否则会产生无限长后缀。
                //也许应该用正则表达式描述其代循环的后缀，但长度很难确定。这是不是K尾文法的一个问题呢？
                se.Push(t.identifier);
                if (t.target.type == "E")
                {
                    string[] rr = se.ToArray();
                    string r = "";
                    for (int i = 0; i < rr.Length; i++) r = rr[i] + r;
                    result.Add(r);                          
                }
                ss.Push(t.target);
                getSuffixes(ref ss, ref se, ref result);             
            }
            ss.Pop();
            se.Pop();

            return;
        }
        //状态合并，单纯地将状态b合并到状态a，后续的孩子状态不合并。
        public void merge(State a, State b)
        {
            if (a == b) return;
            //将指向b的转移变为指向a，将b的转移转加到a上。
            //所有指向b的转移都改为指向a。
            List<State> list = new List<State>();
            getDFSStates(start, ref list);
            foreach (State s in list)
            {
                for (int i = s.transitions.Count - 1; i >= 0; i--)
                {
                    Transition t = s.transitions[i];

                    if (t.target == b)
                    {
                        //若s的转移中不包括指向a的转移，则直接改转移。否则将当前转移标签与原转移标签做或连接。
                        Transition at = s.transitions.FindLast(delegate(Transition tra) { return tra.target == a; });
                        if (at != null)
                        {
                            at.identifier = util.RegxHandler.Or(at.identifier,t.identifier);
                            s.transitions.RemoveAt(i);
                        }
                        else
                            t.target = a;
                    }
                }
            }
            //将状态b的直接后续转移添加到a后面，若a中已存在相同转移，则不要重复添加。
            
            foreach (Transition tb in b.transitions)
            {//对每个tb查找a中相同转移，或没有则在a上添加转移，或有，则区别对待两种情况：目标相同标签不同的转移和目标标签都相同的转移。
                Transition sameT = null;
                foreach (Transition ta in a.transitions)
                {
                    if (ta.target==tb.target)
                    {
                        sameT = ta;
                        break;
                    }
                }
                if (sameT == null) a.transitions.Add(tb);
                else
                {
                    if (sameT.identifier.Equals(tb.identifier)) continue;//同目标，则标签，不需要做任何事情。
                    else//同目标，标签不同，需要或联接。
                    {
                        sameT.identifier =util.RegxHandler.Or(sameT.identifier,tb.identifier);
                    }
                }
            }
        }

        //迭代合并状态：将状态b合并到状态a上面，并将b的孩子与a的孩子进行合并。
        public void mergeIteratively(State a, State b)
        {
            if (a == b) return;
            //将指向b的转移变为指向a，将b的转移转加到a上。
           //所有指向b的转移都改为指向a。
            List<State> list = new List<State>();
            getDFSStates(start, ref list);
            foreach (State s in list)
            {
                for (int i = s.transitions.Count - 1; i >= 0;i-- )
                {
                    Transition t=s.transitions[i];

                    if (t.target == b)
                    {
                        //若s的转移中不包括指向a的转移，则直接改转移。否则将当前转移标签与原转移标签做或连接。
                        Transition at = s.transitions.FindLast(delegate(Transition tra) { return tra.target == a; });
                        if (at != null)
                        {
                            at.identifier = util.RegxHandler.Or(at.identifier, t.identifier);
                            s.transitions.RemoveAt(i);
                        }
                        else
                            t.target = a;
                    }
                }
            }
           //将状态b的后续转移添加到a后面，若a中已存在相同转移，则不要重复添加。
            State m = a;
            State n = b;
            foreach (Transition tb in n.transitions)
            {
                Transition sameT = null;
                foreach (Transition ta in m.transitions)
                {
                    if (ta.identifier.Equals(tb.identifier)) {
                        sameT = ta;
                        break;
                    }
                }
                if (sameT == null) m.transitions.Add(tb);
                else {
                    //m = sameT.target;
                    //n = tb.target;
                    merge(sameT.target, tb.target);
                }
            }
        }
    }
}
