using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class OptimalSet
    {
        public static int ComparewithRelations(List<List<string>> sequences, List<string> relations)
        {
            int indicator = 0;
            foreach (List<string> sequence in sequences)
            {
                int correct_count = 0;
                foreach (string rel in relations)
                {
                    string rel_for_state = rel.Split('!')[0];
                    string rel_back_state = rel.Split('!')[1];
                    int seq_for_state_index = sequence.IndexOf(rel_for_state);
                    int seq_back_state_index = sequence.IndexOf(rel_back_state);
                    if (seq_for_state_index < seq_back_state_index) correct_count++;
                }
                if (correct_count == relations.Count) indicator++;
                //else { foreach (string seq in sequence) Console.Write(seq + "\t"); Console.WriteLine(); }
            }
            if (indicator == sequences.Count) return 1;
            else return 0;
        }
        //与最优集对比并进行评分
        public static double[] ComparewithSet(List<List<string>> bestset, List<List<string>> methodset)
        {
            int intersection = 0;
            double[] score = new double[3] { 0, 0, 0 };
            foreach (List<string> best_seq in bestset)
            {
                foreach (List<string> method_seq in methodset)
                {
                    int correct_count = 0;
                    for (int i = 0; i < method_seq.Count; i++)
                        if (best_seq[i].Equals(method_seq[i]))
                            correct_count++;
                    if (correct_count == method_seq.Count)
                    {
                        intersection++;
                        break;
                    }
                }
            }

            if (intersection == bestset.Count && bestset.Count == methodset.Count) score[0] = 4;
            else if (intersection == methodset.Count && bestset.Count > methodset.Count) score[0] = 3;
            else if (intersection == bestset.Count && bestset.Count < methodset.Count) score[0] = 2;
            else if (intersection == 0) score[0] = 0;
            else score[0] = (intersection / (double)methodset.Count);
            score[1] = (intersection / (double)methodset.Count);    //precision
            score[2] = (intersection / (double)bestset.Count);      //recall


            return score;
        }

        // 判断一个数组内元素是否降序排列
        public static bool isDesc(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
                if (array[i] > array[i - 1])
                    return false;
            return true;
        }

        //根据权重值和全排列生成状态消减序列集合
        public static List<List<string>> GeneratedSequencesbyFullArrangement(List<int> weight)
        {
            List<string> array = new List<string>();
            List<string> error_order = new List<string>();
            List<List<int>> sequences = new List<List<int>>();
            List<List<string>> output = new List<List<string>>();

            //1.复制一个新数组：修改时在临时数组中修改
            int[] temp = new int[weight.Count - 2];
            for (int i = 0; i < weight.Count - 2; i++)
                temp[i] = i + 1;
            //2.将第一个序列输入到传递闭包法
            sequences.Add(temp.ToList());
            //3.依次寻找并打印全排序
            while (!isDesc(temp))
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

                sequences.Add(temp.ToList());
            }

            for (int k = 0; k < sequences.Count; k++)
            {
                bool isContain = true;
                for (int i = 0; i < weight.Count - 2; i++)
                {
                    for (int j = i + 1; j < weight.Count - 2; j++)
                    {
                        int for_state = sequences[k][i];
                        int back_state = sequences[k][j];
                        if (weight[for_state] > weight[back_state])
                        {
                            isContain = false;
                            break;
                        }
                    }
                    if (i == weight.Count - 3)
                    {
                        if (weight[sequences[k][i]] != weight.Max())
                            isContain = false;
                    }
                    if (!isContain) break;
                }
                if (!isContain) sequences.RemoveAt(k--);
            }

            foreach (List<int> sequence in sequences)
            {
                List<string> temp_seq = new List<string>();
                foreach (int state in sequence)
                    temp_seq.Add(state.ToString());
                temp_seq.Add("0");
                temp_seq.Add((weight.Count - 1).ToString());
                output.Add(temp_seq.ToList());
            }
            return output;
        }

        //根据权重值生成状态消减序列集合
        public static List<List<string>> GeneratedSequencesbyWeight(List<int> weight)
        {
            List<int[]> weight_sort = new List<int[]>();
            List<List<int>> sequences = new List<List<int>>();
            List<List<string>> output = new List<List<string>>();

            int times = weight.Count - 2;
            int[] read = new int[times];
            for (int i = 0; i < times; i++)
            {
                read[i] = -1;
                weight_sort.Add(new int[2] { i + 1, weight[i + 1] });
            }
            weight_sort = weight_sort.OrderBy(u => u[1]).ToList();
            for (int k = 0; k < times; k++)
            {
                if (read.Contains(weight_sort[k][0])) continue;
                int isduplicated = weight_sort.Where(g => g[1] == weight_sort[k][1]).Count();
                if (isduplicated > 1)
                {
                    int seq_count = sequences.Count;
                    int[] dup_state = new int[isduplicated];
                    List<List<int>> dup_seq = new List<List<int>>();
                    for (int i = k, j = 0; i < times; i++)
                        if (weight_sort[i][1] == weight_sort[k][1])
                            dup_state[j++] = weight_sort[i][0];
                    for (int i = 0, j = 0; i < times && j < isduplicated; i++)
                        if (read[i] == -1 && dup_state[j] != 0)
                            read[i] = dup_state[j++];

                    //依次寻找并打印全排序
                    dup_seq.Add(dup_state.ToList());
                    while (!isDesc(dup_state))
                    {
                        //1.找出数组的最大值
                        int max = dup_state.Max();

                        //2.从后向前找：找到第一组后数大于前数，以后数位置为signer
                        int signer = dup_state.Length - 1;
                        for (int i = dup_state.Length - 1; i > 0; i--)
                            if (dup_state[i] > dup_state[i - 1]) { signer = i; break; }

                        //3.从signer向后找：找到大于且最接近于array[signer-1]的数array[t]
                        int t = signer;
                        for (int i = signer; i < dup_state.Length; i++)
                        {
                            if (dup_state[i] > dup_state[signer - 1] && dup_state[i] < max)
                            {
                                t = i;
                                max = dup_state[t];
                            }
                        }

                        //4.将找到的array[t]和array[signer-1]互换
                        int temp1 = dup_state[t];
                        dup_state[t] = dup_state[signer - 1];
                        dup_state[signer - 1] = temp1;

                        //5.为signer之后的元素升序排序
                        for (int i = signer; i < dup_state.Length; i++)
                        {
                            for (int j = i + 1; j < dup_state.Length; j++)
                                if (dup_state[i] > dup_state[j])
                                {
                                    temp1 = dup_state[i];
                                    dup_state[i] = dup_state[j];
                                    dup_state[j] = temp1;
                                }
                        }
                        dup_seq.Add(dup_state.ToList());
                    }
                    //序列集复制
                    for (int i = 0; i < dup_seq.Count - 1; i++)
                    {
                        if (seq_count == 0) break;
                        else
                            for (int j = 0; j < seq_count; j++)
                                sequences.Add(sequences[j].ToList());
                    }
                    //等权值元素复制
                    int dup_seq_count = dup_seq.Count; 
                    for (int i = 0; i < seq_count - 1; i++)
                    {
                        for (int j = 0; j < dup_seq_count; j++)
                            dup_seq.Add(dup_seq[j].ToList());
                    }

                    if (sequences.Count == 0)
                        for (int i = 0; i < dup_seq.Count; i++)
                            sequences.Add(dup_seq[i].ToList());
                    else
                        for (int i = 0; i < sequences.Count; i++)
                            sequences[i].AddRange(dup_seq[i].ToList());

                }
                else
                {
                    if (sequences.Count == 0)
                        sequences.Add(new List<int>() { weight_sort[k][0] });
                    else
                    {
                        for (int i = 0; i < sequences.Count; i++)
                            sequences[i].Add(weight_sort[k][0]);
                    }
                }
            }

            for (int i = 0; i < sequences.Count; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < times; j++)
                    temp.Add(sequences[i][j].ToString());
                temp.Add("0");
                temp.Add((times + 1).ToString());
                output.Add(temp.ToList());
            }

            return output;
        }

        //根据权重值生成状态消减序列集合
        public static List<string> GeneratedOrderbyWeight(List<int> weight)
        {
            List<int[]> weight_sort = new List<int[]>();
            List<List<int>> sequences = new List<List<int>>();
            List<string> output = new List<string>();

            int times = weight.Count - 2;
            int[] read = new int[times];
            for (int i = 0; i < times; i++)
            {
                read[i] = -1;
                weight_sort.Add(new int[2] { i + 1, weight[i + 1] });
            }
            weight_sort = weight_sort.OrderBy(u => u[1]).ToList();
            foreach (int[] arr in weight_sort)
                output.Add(arr[0].ToString());
            output.Add("0");
            output.Add((times + 1).ToString());

            return output;
        }

        //根据序列集合生成规范消减序列
        public static List<string> GeneratewithSet (List<List<string>> methodset)
        {
            int iSeed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            Random rd = new Random(iSeed);
            List<string> temp = methodset[rd.Next(0, methodset.Count)].ToList();
            List<string> output = new List<string>();
            foreach (string state in temp)
            {
                if (state.Equals("0")) output.Add("S0");
                else if (state.Equals((temp.Count - 1).ToString()))
                    output.Add("E" + state);
                else output.Add("M" + state);
            }
            return output.ToList();
        }

        public static List<string> Generatewithoutstring(List<List<string>> methodset)
        {
            int iSeed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            Random rd = new Random(iSeed);
            List<string> temp = methodset[rd.Next(0, methodset.Count)].ToList();
            List<string> output = new List<string>();
            foreach (string state in temp)
            {
                if (state.Equals("0")) output.Add("0");
                else if (state.Equals((temp.Count - 1).ToString()))
                    output.Add(state);
                else output.Add(state);
            }
            return output.ToList();
        }
    }
}
