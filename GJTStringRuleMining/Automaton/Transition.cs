using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//说明：转移
namespace MZQStringRuleMining.Automaton
{
    public class Transition : IEquatable<Transition>
    {
        public string identifier; //用单个字符
        string description;
        public State target=new State("End"); //目标状态

        public Transition(string arg)
        {
            identifier = arg;
        }
        //克隆一个转移
        public Transition clone(StateMachine sm)
        {
            Transition t = new Transition(identifier);
            t.description = description;
            List<State> list=new List<State>();
            State search_state=null;
                

            foreach (State s in sm.stateList)
            {
                if (s.identifier.Equals(target.identifier)) search_state = s;
            }

            if(search_state==null) t.target = target.clone(sm);
            else t.target=search_state;
            return t;
        }
        //重写equal方法
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Transition objAsTra = obj as Transition;
            if (objAsTra == null) return false;
            else return Equals(objAsTra);
        }
        //接口函数
        public bool Equals(Transition transition)
        {
            if (transition == null) return false;
            return (this.target.Equals(transition.target));
        }
    }
}
