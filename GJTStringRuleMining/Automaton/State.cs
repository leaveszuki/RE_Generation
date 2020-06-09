using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//说明：状态
namespace MZQStringRuleMining.Automaton
{
    public class State:Object, IEquatable<State>
    {
        public string identifier; //单个字符
        public string type;//S:初态；M：中间态；E:终态
        public string description;
        public List<Transition> transitions=new List<Transition>(); //转移列表

        public State(string arg)
        {
            identifier = arg;
            type = "M";//默认是中间态
        }

        //克隆一个状态
        public State clone(StateMachine sm)
        {
            State s = new State(this.identifier);
            s.description = description;
            s.transitions = new List<Transition>();
            s.type = type;
            if (!sm.stateList.Contains(s))
                sm.stateList.Add(s);
            foreach (Transition t in transitions)
            {
                s.transitions.Add(t.clone(sm));
            }
           return s;
        }
        //重写Equal方法
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            State objasState = obj as State;
            if (objasState == null) return false;
            else return Equals(objasState);
        }
        //接口函数
        public bool Equals(State state)
        {
            if (state == null) return false;
            return (this.identifier.Equals(state.identifier));
        }
    }

    class DirectNec : Object    //直接前后必经结点集合
    {
        public string identifier = "";  //非桥结点
        public string forthnec = "";   //前必经结点
        public string backnec = "";    //后必经结点

        public DirectNec clone()
        {
            DirectNec dn = new DirectNec();

            dn.identifier = identifier;
            dn.forthnec = forthnec;
            dn.backnec = backnec;
            return dn;
        }
    }
}
