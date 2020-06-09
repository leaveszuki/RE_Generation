using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class Construct
    {

        //自动机编码的进制转换
        public static string CodeConvert(int times, int in_convert, int out_convert, string code)
        {

            string bin_code = "";
            double bit_len = times * times * Math.Log(2, out_convert);
            int[] input = new int[4096];    //保存M进制下的各个位数，最大需保存十进制下2^(100^2)位数
            int[] output = new int[32768];   //保存N进制下的各个位数，最大需保存二进制下12042位数
            int k = 0;
            int len = code.Length;
            Array.Clear(output, 0, output.Length);

            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] > '9') input[i] = code[i] - 'A' + 10;
                else input[i] = code[i] - '0';
            }

            for (int sum = 1; sum > 0;)
            {
                sum = 0;
                int d = 0;
                for (int i = 0; i < len; i++)
                {
                    d = input[i] / out_convert;
                    sum += d;
                    if (i == len - 1) output[k++] = input[i] % out_convert;
                    else input[i + 1] += (input[i] % out_convert) * in_convert;
                    input[i] = d;
                }
            }
            if (k == 0) { output[k] = 0; k--; }
            if (k == -1) return null;
            else
            {
                for (int i = 0; i < k; i++)
                {
                    if (output[k - i - 1] > 9) bin_code += (output[k - i - 1] + 'a' - 10).ToString();
                    else bin_code += output[k - i - 1].ToString();
                }
            }
            while (bin_code.Length < Math.Round(bit_len)) bin_code = "0" + bin_code;
            //Console.Write(bin_code.ToString() + "\n");
            return bin_code;
        }

        /*  功能说明：生成四阶自动机（用字符标识事件）
         *  输入参数：自动机的阶数和边的编码
         *  返回参数：文本格式的字符串（每行格式为：输入状态\t1\t输出状态）
         *  备    注：button4调用
         */
        public static List<string> GeneratewithCode(int times, string bin_code)
        {
            char[] arr = bin_code.ToCharArray();
            Array.Reverse(arr);
            string rever_code = new string(arr);
            List<string> inputstrings = new List<string>();
            inputstrings.Add("S0\t!\tM1");
            for (int idx = 0; idx < rever_code.Length; idx++)
            {
                if (rever_code[idx] == '1')
                {
                    int start = (idx / times) + 1;
                    int end = idx % times + 1;
                    byte[] asc_arr = new byte[] { (byte)(Convert.ToInt32(96 + end)) };
                    string str = Convert.ToString(System.Text.Encoding.ASCII.GetString(asc_arr));
                    inputstrings.Add("M" + start + "\t" + str + "\tM" + end);
                }
            }
            inputstrings.Add("M" + times + "\t!\tE" + (times + 1));
            return inputstrings;
        }
        
        //生成自动机
        //条件：是否无环；是否随机
        //返回值：自动机，二进制的自动机编码
        public static StateMachine GenerateAutomata(int times, ref string code, bool iscycle)
        {
            StateMachine outputautomata = new StateMachine();
            List<string> inputstrings = new List<string>();
            string temp_code = "";
            if (!code.Equals(""))
            {
                temp_code = CodeConvert(times, 10, 2, code);

                bool flag = false;
                StateMachine temp_automata = new StateMachine();
                Adjacency(times, temp_code, ref flag, ref temp_automata);
                if (!flag) return null;

                inputstrings = GeneratewithCode(times, temp_code);
                outputautomata.InsertAutomata(inputstrings);
            }
            else
            {
                Random rd = new Random(Guid.NewGuid().GetHashCode());
                int str_len = times * times;
                bool flag = false;
                while (!flag)
                {
                    temp_code = "";
                    StateMachine temp_automata = new StateMachine();
                    if (iscycle) //编写有环自动机
                    {
                        for (int i = 0; i < str_len; i++)
                            temp_code += rd.Next(0, 2) >= 0.5 ? "1" : "0";
                    }
                    else    //编写无环自动机(无等号则允许自身环)
                    {
                        for (int i = str_len - 1; i >= 0; i--)
                        {
                            if ((i / times) >= (i % times))
                                temp_code += "0";
                            else if ((i / times + 1) == (i % times))
                                temp_code += "1";
                            else
                            {
                                temp_code += rd.Next(0, 2) >= 1 ? "1" : "0";
                            }

                        }
                    }

                    Adjacency(times, temp_code, ref flag, ref temp_automata);
                    if (flag) outputautomata = temp_automata.clone();
                }
            }
            code = string.Copy(temp_code);
            return outputautomata.clone();
        }

        //验证该编码对应的图是否可达，若可达，生成邻接矩阵和邻接表
        public static string[,] Adjacency(int times, string code, ref bool flag, ref StateMachine m)
        {
            string bin_code = code;

            List<string> inputstrings = new List<string>();
            List<State> l_states = new List<State>();

            inputstrings = GeneratewithCode(times, bin_code);

            m.InsertAutomata(inputstrings);
            m.getDFSStates(m.start, ref l_states);

            int count_States = l_states.Count;
            string[,] Ri = new string[times + 2, times + 2];
            int[,] Rj = new int[times + 2, times + 2];

            for (int x = 0; x < times + 2; x++)
                for (int y = 0; y < times + 2; y++)
                {
                    Ri[x, y] = "";
                    Rj[x, y] = 65535;
                }

            foreach (State s in l_states)
            {
                foreach (Transition t in s.transitions)
                {
                    int i = Int32.Parse(s.identifier.Substring(1));
                    int j = Int32.Parse(t.target.identifier.Substring(1));
                    Ri[i, j] = t.identifier;
                    Rj[i, j] = t.identifier.Length;
                }
            }

            for (int k = 0; k < times + 2; k++)
                for (int i = 0; i < times + 2; i++)
                    for (int j = 0; j < times + 2; j++)
                        if (Rj[i, j] > (Rj[i, k] + Rj[k, j]))
                            Rj[i, j] = Rj[i, k] + Rj[k, j];
            if (Rj[0, times + 1] == 65535) flag = false;
            else flag = true;
            return Ri;
        }
    }
}
