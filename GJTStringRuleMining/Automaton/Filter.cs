using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class Filter
    {
        public static bool FilterMethod(int times, List<string> code, List<double> length, List<List<List<string>>> best)
        {

            double test_len = length.Last();
            List<List<string>> test_best = best.Last().ToList();
            string bin_code = Construct.CodeConvert(times, 10, 2, code.Last());
            int degree = 0;
            int[,] test_inout = new int[times, 2];
            for (int i = 0; i < times; i++) { test_inout[i, 0] = 0; test_inout[i, 1] = 0; }
            for (int i = 0; i < times; i++)
                for (int j = 0; j < times; j++)
                    if (bin_code[i * times + j] == '1')
                    {
                        test_inout[times - 1 - i, 0]++;
                        test_inout[times - 1 - j, 1]++;
                        degree++;
                    }

            int[,] location_count = new int[times, times];
            for (int i = 0; i < times; i++) for (int j = 0; j < times; j++) location_count[i, j] = 0;
            foreach (List<string> seq in test_best)
                for (int i = 0; i < times; i++)
                {
                    int state_num = Convert.ToInt32(seq[i]) - 1;
                    location_count[state_num, seq.IndexOf(seq[i])]++;
                }

            foreach (string control in code)
            {
                if (control.Equals(code.Last())) continue;

                int score = 0;
                //两个自动机的度相等；
                int deg_con = 0;
                string bin_control = Construct.CodeConvert(times, 10, 2, control);
                foreach (char c in bin_control) if (c == '1') deg_con++;
                if (degree == deg_con) score++;
                else continue;

                //A_1中任意状态的出度和入度总等于A_2中某个状态的出度和入度；
                int[,] con_inout = new int[times, 2];
                List<int> test_record = new List<int>();
                List<int> con_record = new List<int>();
                for (int i = 0; i < times; i++) { con_inout[i, 0] = 0; con_inout[i, 1] = 0; }
                for (int i = 0; i < times; i++)
                    for (int j = 0; j < times; j++)
                        if (bin_control[i * times + j] == '1')
                        {
                            con_inout[times - 1 - i, 0]++;
                            con_inout[times - 1 - j, 1]++;
                        }
                for (int i = 0; i < times; i++)
                    for (int j = 0; j < times; j++)
                        if (test_inout[i, 0] == con_inout[j, 0] && test_inout[i, 1] == con_inout[j, 1])
                        {
                            if (!test_record.Contains(i) && !con_record.Contains(j))
                            {
                                test_record.Add(i);
                                con_record.Add(j);
                            }
                        }
                if (test_record.Count == con_record.Count && con_record.Count == times) score++;
                else continue;

                if (test_record[0] == con_record[0] && test_record.Last() == con_record.Last())
                    score++;
                else continue;

                //两个自动机的最优集相似
                int set_count = 0;
                int[,] con_location = new int[times, times];
                for (int i = 0; i < times; i++) for (int j = 0; j < times; j++) con_location[i, j] = 0;
                foreach (List<string> seq in best[code.IndexOf(control)])
                    for (int i = 0; i < times; i++)
                    {
                        int state_num = Convert.ToInt32(seq[i]) - 1;
                        con_location[state_num, seq.IndexOf(seq[i])]++;
                    }
                for (int i = 0; i < times; i++)
                {
                    int temp_count = 0;
                    for (int j = 0; j < times; j++)
                        if (location_count[test_record[i], j] == con_location[con_record[i], j])
                            temp_count++;
                    if (temp_count == times) set_count++;
                }
                if (set_count == times)
                    score++;
                else
                    continue;

                if (score == 4)
                {
                    if (length[code.IndexOf(control)] == test_len)
                        return true;
                    else
                    {
                        Console.WriteLine(code.Last() + "\t" + control);
                        break;
                    }
                }
            }

            return false;
        }

        public static bool GenerateSimilarAutomata(int times, string code, ref List<List<string>>similar_list)
        {
            foreach (List<string> code_set in similar_list)
                if (code_set.Contains(code))
                    return false;

            List<string> similar_codes = new List<string>();
            similar_codes.Add(code);
            //复制一个新数组：修改时在临时数组中修改
            int[] temp = new int[times - 2];
            for (int i = 0; i < times - 2; i++)
                temp[i] = i + 1;

            //3.依次寻找并打印全排序
            while (!OptimalSet.isDesc(temp))
            {
                //1.找出数组的最大值
                int max = temp.Max();

                //2.从后向前找：找到第一组后数大于前数，以后数位置为signer
                int signer = temp.Length - 1;
                for (int i = temp.Length - 1; i > 0; i--)
                    if (temp[i] > temp[i - 1]) { signer = i; break; }

                //3.从signer向后找：找到大于且最接近于array[signer-1]的数array[t]
                int t = signer;
                for (int i = signer; i < temp.Length; i++)
                {
                    if (temp[i] > temp[signer - 1] && temp[i] < max)
                    {
                        t = i;
                        max = temp[t];
                    }
                }

                //4.将找到的array[t]和array[signer-1]互换
                int temp1 = temp[t];
                temp[t] = temp[signer - 1];
                temp[signer - 1] = temp1;

                //5.为signer之后的元素升序排序
                for (int i = signer; i < temp.Length; i++)
                {
                    for (int j = i + 1; j < temp.Length; j++)
                        if (temp[i] > temp[j])
                        {
                            temp1 = temp[i];
                            temp[i] = temp[j];
                            temp[j] = temp1;
                        }
                }

                string new_code = GenerateSimilarCode(times, code, temp);
                if (!similar_codes.Contains(new_code)) similar_codes.Add(new_code);
            }

            similar_list.Add(similar_codes);
            return true;

        }

        public static string GenerateSimilarCode(int times, string code, int[] order)
        {
            string output_code = null;
            string output_bin_code = null;
            char[,] matrix = new char[times, times];
            string input_code = Construct.CodeConvert(times, 10, 2, code);

            for (int x = 0; x < times; x++)
                for (int y = 0; y < times; y++)
                    matrix[times - 1 - x, times - 1 - y] = input_code[x * times + y];

            for (int i = 0; i < order.Length; i++)
            {
                int state = order[i];
                int before_number = i + 1;

                //行变换
                for (int y = 0; y < times; y++)
                {
                    char temp = matrix[before_number, y];
                    matrix[before_number, y] = matrix[state, y];
                    matrix[state, y] = temp;
                }

                //列变换
                for (int y = 0; y < times; y++)
                {
                    char temp = matrix[y, before_number];
                    matrix[y, before_number] = matrix[y, state];
                    matrix[y, state] = temp;
                }
            }

            for (int x = 0; x < times; x++)
                for (int y = 0; y < times; y++)
                    output_bin_code += matrix[times - 1 - x, times - 1 - y];

            output_code = Construct.CodeConvert(times, 2, 10, output_bin_code);
            return output_code;
        }

        public static List<string> AcycleAutomataSet(int times)
        {
            List<string> code_set = new List<string>();
            int bit = (times - 2) * (times - 1) / 2;
            int start = (times - 2) * (times - 2) - bit;

            for (int i = 0; i < Math.Pow(2,bit); i++)
            {
                string new_code = null;
                string bin_code = Construct.CodeConvert(times - 2, 10, 2, i.ToString());
                bin_code = bin_code.Substring(start);
                for (int j = 0, k = 1, u = 0; j < times * times; j++)
                    if (j == k * times + k - 1) { new_code += "1"; k++; }
                    else if (j >= k * times && j < k * times + k - 1)
                        new_code += bin_code[u++];
                    else new_code += "0";
                string code = Construct.CodeConvert(times, 2, 10, new_code);
                code_set.Add(code);
            }

            return code_set;
        }
    }
}
