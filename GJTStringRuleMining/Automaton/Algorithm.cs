using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class Algorithm
    {
        
        public static void GenerateGraph(int times, StateMachine automata, string name)
        {
            //string bin_code = Construct.CodeConvert(times, 10, 2, str_code);
            string outputfilename = (name + ".jff");
            List<string> InputStrings = new List<string>();
            List<double[]> coordinates = new List<double[]>();
            int centroid = 1000 + 100 * times;
            int halfdiagonal = 50 * times;
            int tran_len = 4 + times + 2 + 1 + 1;
            //根据形心，形心到顶点的距离，顶点数量生成正多边形
            coordinates.Add(new double[2] { centroid - 2 * halfdiagonal, centroid });       //初态坐标
            for (int i = 0; i < times; i++)
            {
                double x = 0, y = 0;
                double angle = 2 * Math.PI / times * i;
                x = centroid - halfdiagonal * Math.Cos(angle);
                y = centroid + halfdiagonal * Math.Sin(angle);
                coordinates.Add(new double[2] { x, y });
            }
            coordinates.Add(new double[2] { centroid - 2 * halfdiagonal, centroid - halfdiagonal });   //终态坐标

            //注入固定的文本信息
            InputStrings.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><!--Created with JFLAP 6.4.--><structure>&#13;\n");
            InputStrings.Add("\t<type>fa</type>&#13;\n");
            InputStrings.Add("\t<automaton>&#13;\n");
            //注入顶点信息
            InputStrings.Add("\t<!--The list of states.-->&#13;\n");
            foreach (double[] x_y in coordinates)
            {
                int index = coordinates.IndexOf(x_y);
                string temp = "\t<state id=\"";
                temp += index.ToString();
                temp += "\" name=\"q";
                temp += index.ToString();
                temp += "\">&#13; <x>";
                temp += Math.Round(x_y[0], 1).ToString();
                temp += "</x>&#13; <y>";
                temp += Math.Round(x_y[1], 1).ToString();
                temp += "</y>&#13; ";
                if (index == 0) temp += "<initial/>&#13; ";
                else if (index == times + 1) temp += "<final/>&#13; ";
                temp += "</state>&#13;\n";
                InputStrings.Add(temp.Clone().ToString());
            }
            //注入转移关系信息
            InputStrings.Add("\t<!--The list of transitions.-->&#13;\n");
            InputStrings.Add("\t<transition>&#13; <from>0</from>&#13; <to>1</to>&#13; <read/>&#13; </transition>&#13;\n");
            for (int i = 1; i <= times; i++)
                for (int j = 1; j <= times; j++)
                {
                    string temp = "\t<transition>&#13; <from>";
                    temp += i.ToString();
                    temp += "</from>&#13; <to>";
                    temp += j.ToString();
                    temp += "</to>&#13; <read>!</read>&#13; </transition>&#13;\n";
                    InputStrings.Add(temp.Clone().ToString());
                }
            {
                string temp = "\t<transition>&#13; <from>";
                temp += times.ToString();
                temp += "</from>&#13; <to>";
                temp += (times + 1).ToString();
                temp += "</to>&#13; <read/>&#13; </transition>&#13;\n";
                InputStrings.Add(temp.Clone().ToString());
            }

            //注入固定的文本信息
            InputStrings.Add("\t</automaton>&#13;\n");
            InputStrings.Add("</structure>\n");
            for (int i = 8 + times; i < InputStrings.Count - 3; i++)
            {
                string from = ((i - 8 - times) / times + 1).ToString();
                string to = ((i - 8 - times) % times + 1).ToString();
                bool ischanged = false;
                foreach (State s in automata.stateList)
                {
                    if (!s.identifier.Substring(1).Equals(from)) continue;
                    foreach (Transition t in s.transitions)
                    {
                        if (!t.target.identifier.Substring(1).Equals(to)) continue;
                        else
                        {
                            string[] temp = InputStrings[i].Split('!');
                            string inputstring = temp[0] + t.identifier + temp[1];
                            InputStrings[i] = inputstring;
                            ischanged = true;
                        }

                    }
                }
                if (!ischanged)
                    InputStrings[i] = "<!--" + InputStrings[i] + "-->";
            }

            FileStream fs = new FileStream(outputfilename, FileMode.Create, FileAccess.Write);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            foreach (string str in InputStrings) { sw.Write(str); sw.WriteLine(); }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static StateMachine DeleteRedundancy(StateMachine automata)
        {
            StateMachine m = automata.clone();
            List<State> nec_states = m.getDominatorSequence();
            int[] found_in = new int[Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1];
            int[] found_out = new int[Convert.ToInt16(m.getEndState()[0].identifier.Substring(1)) + 1];
            for (int i = 0; i < found_in.Length; i++) { found_in[i] = 0; found_out[i] = 0; }
            for (int i = 0; i < m.stateList.Count; i++)
            {
                List<State> l_states = new List<State>();
                automata.getDFSStates(m.stateList[i], ref l_states);
                if (nec_states.Contains(m.stateList[i]))
                {
                    foreach (State state in l_states)
                    {
                        int num_state = Convert.ToInt16(state.identifier.Substring(1));
                        found_in[num_state]++;
                    }
                }
                else
                {
                    if (l_states.Count != 0)
                    {
                        int num_state = Convert.ToInt16(m.stateList[i].identifier.Substring(1));
                        foreach (State state in l_states)
                            if (nec_states.Contains(state))
                            {
                                found_out[num_state]++;
                                break;
                            }
                    }
                }

            }

            for (int i = 1; i < found_in.Length - 1; i++)
            {
                bool isnec = false;
                foreach (State state in nec_states) if (state.identifier.Equals("M" + i)) isnec = true;
                if (isnec) continue;
                if (found_in[i] * found_out[i] == 0)
                    for (int j = 0; j < m.stateList.Count; j++)
                        if (m.stateList[j].Equals("M" + i)) m.stateList.RemoveAt(j--);
                        else
                            for (int k = 0; k < m.stateList[j].transitions.Count; k++)
                                if (m.stateList[j].transitions[k].target.identifier.Equals("M" + i))
                                    m.stateList[j].transitions.RemoveAt(k--);
            }

            return m;
        }
    }
}
