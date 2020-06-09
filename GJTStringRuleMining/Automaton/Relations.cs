using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining.Automaton
{
    class Relations
    {
        //根据对暴力搜索的剪枝生成标准隶属关系集合
        public static List<string> StandardSubjection(List<List<string>> sequences)
        {
            List<string> relations = new List<string>();

            //根据第一条消减序列产生各个状态间的隶属关系，称之为源隶属关系
            for (int i = 0; i < sequences[0].Count - 1; i++)
                for (int j = i + 1; j < sequences[0].Count; j++)
                    relations.Add((sequences[0][i] + "!" + sequences[0][j]).ToString());

            //根据之后的消减序列删除与源不同的隶属关系
            for (int i = 1; i < sequences.Count; i++)
            {
                for (int j = 0; j < sequences[i].Count - 1; j++)
                {
                    string new_rel = sequences[i][j] + "!" + sequences[i][j + 1];
                    if (relations.Contains(new_rel))
                        continue;
                    else
                    {
                        string old_rel = sequences[i][j + 1] + "!" + sequences[i][j];
                        relations.Remove(old_rel);
                    }

                }
            }

            //隶属关系化简
            for (int i = 0; i < relations.Count - 1; i++)
                for (int j = i + 1; j < relations.Count; j++)
                {
                    string[] forth_states = relations[i].Split('!');
                    string[] back_states = relations[j].Split('!');
                    if (forth_states[1].Equals(back_states[0]))
                    {
                        string test_rel = forth_states[0] + "!" + back_states[1];
                        if (relations.Contains(test_rel))
                        {
                            relations.Remove(test_rel);
                            j--;
                        }
                    }
                }

            //根据隶属关系中各个状态的频数生成权重值
            //int[] weight = new int[sequences[0].Count];
            foreach (string data in relations) Console.Write(data + "\n");
            Console.WriteLine();
            //List<int> output = new List<int>(weight.ToList());

            return relations.ToList();
        }

        //快速生成隶属关系集合
        public static List<string> GenerateSubjection(List<List<string>> sequences)
        {
            List<string> relations = new List<string>();
            List<string> equal_relations = new List<string>();

            for (int i = 0; i < sequences[0].Count - 1; i++)
                relations.Add(sequences[0][i] + "!" + sequences[0][i + 1]);

            for (int i = 1; i < sequences.Count; i++)
            {
                List<string> adding_relations = new List<string>();
                for (int j = 0; j < sequences[i].Count - 1; j++)
                {
                    string new_rel = sequences[i][j] + "!" + sequences[i][j + 1];
                    if (!relations.Contains(new_rel)) adding_relations.Add(new_rel);
                }

                for (int j = 0; j < adding_relations.Count; j++)
                {
                    string[] test_rel = adding_relations[j].Split('!');
                    string new_rel = test_rel[1] + "!" + test_rel[0];
                    if (equal_relations.Contains(adding_relations[j]) || equal_relations.Contains(new_rel))
                    {
                        adding_relations.RemoveAt(j--);
                        continue;
                    }
                    if (relations.Contains(new_rel))
                    {
                        relations.Remove(new_rel);
                        adding_relations.RemoveAt(j--);
                        equal_relations.Add(new_rel);
                    }
                }

                foreach (string str in adding_relations) relations.Add(str);
            }

            return relations;
        }

        //根据隶属关系比较权重
        public static int RelationsCompare(List<int> weight, List<string> relations, ref List<string> weight_rel)
        {
            int contains_count = 0;
            int end_state = 0;
            List<int[]> weight_sort = new List<int[]>();
            for (int i = 0; i < weight.Count; i++) weight_sort.Add(new int[] { i, weight[i] });
            weight_sort = weight_sort.OrderBy(u => u[1]).ToList();
            for (int i = 1; i < weight_sort.Count; i++)
            {
                if (weight_sort[i][1] < weight_sort[i + 1][1])
                {
                    end_state = weight_sort[i][0];
                    break;
                }
            }
            for (int i = 1; i < weight_sort.Count - 1; i++)
            {
                if (weight_sort[i][0] == end_state) continue;
                for (int j = i + 1; j < weight_sort.Count; j++)
                    if (weight_sort[i][1] < weight_sort[j][1])
                    {
                        weight_rel.Add(weight_sort[i][0] + "!" + weight_sort[j][0]);
                        for (int k = j + 1; k < weight_sort.Count; k++)
                        {
                            if (weight_sort[j][1] == weight_sort[k][1])
                                weight_rel.Add(weight_sort[i][0] + "!" + weight_sort[k][0]);
                            else
                                break;
                        }
                        break;
                    }
            }

            for (int i = weight_sort.Count - 1; i > 0; i--)
            {
                if (i < weight_sort.Count - 1)
                    if (weight_sort[i][1] < weight_sort[i + 1][1])
                        break;
                weight_rel.Add(weight_sort[i][0] + "!0");
            }
            weight_rel.Add("0!" + end_state);

            foreach (string rel in weight_rel)
            {
                string[] rel_split = rel.Split('!');
                string reverse_rel = rel_split.Last() + "!" + rel_split.First();
                if (relations.Contains(rel)) contains_count++;
                if (relations.Contains(reverse_rel)) return 0;
            }

            if (contains_count == weight.Count) return 2;

            if (contains_count == 0) return 0;

            return 1;
        }

        //检测前序关系生成的自动机是否包含矛盾关系
        public static bool ContainsCycle(List<string> relations)
        {
            List<string> inputstrings = new List<string>();
            List<int[]> splitstring = new List<int[]>();
            StateMachine machine = new StateMachine();
            int end_state = 0;
            foreach (string str in relations)
            {
                int[] temp = new int[2];
                temp[0] = Convert.ToInt16(str.Split('!')[0]);
                temp[1] = Convert.ToInt16(str.Split('!')[1]);
                splitstring.Add(temp.ToArray());
                if (temp[0] == 0 && temp[1] > end_state) end_state = temp[1];
            }

            int INF = 65535;
            int state_count = end_state + 1;
            int[,] Ri = new int[state_count, state_count];
            int[,] Rj = new int[state_count, state_count];
            for (int i = 0; i < state_count; i++)
                for (int j = 0; j < state_count; j++)
                    Ri[i, j] = Rj[i, j] = INF;
            foreach (int[] temp in splitstring)
                Ri[temp[0], temp[1]] = 1;

            for (int k = 0; k < state_count; k++)
            {
                for (int i = 0; i < state_count; i++)
                    for (int j = 0; j < state_count; j++)
                        if (Ri[i, j] > Ri[i, k] + Ri[k, j])
                            Rj[i, j] = Ri[i, k] + Ri[k, j];
                        else Rj[i, j] = Ri[i, j];

                for (int i = 0; i < state_count; i++)
                    for (int j = 0; j < state_count; j++)
                    {
                        if (i == j)
                            Ri[i, j] = INF;
                        else
                            Ri[i, j] = Rj[i, j];
                        Rj[i, j] = INF;
                    }
            }

            for (int i = 0; i < state_count; i++)
                for (int j = 0; j < state_count; j++)
                    if (Ri[i, j] < INF && Ri[j, i] < INF)
                        return false;

            return true;
        }
    }
}
