using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MZQStringRuleMining.Automaton;

namespace MZQStringRuleMining
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        //实验五：各启发式算法和权重法的长度比较
        private void button1_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            List<List<string>> zerob_weight = TimesDegree.DynamicTimesDegree(m.clone());
            List<string> order = OptimalSet.Generatewithoutstring(zerob_weight);

            textBox5.Text += "动态度乘积法：";
            textBox5.Text += DFA2RE.TransitiveClosureMethod(m.clone(), order);
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;
        }

        //主要功能
        //实验二：起始序列实验
        private void button2_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            Stack<State> stack = new Stack<State>();
            List<State> nec_path = new List<State>();
            List<string> order = NeccessaryPath.getStateEliminationSequenceBasedonNecessaryPath(m.clone(), ref stack, ref nec_path);
            for (int idx = 0; idx < order.Count; idx++) order[idx] = order[idx].Substring(1);

            textBox5.Text += "桥状态法：";
            textBox5.Text += DFA2RE.TransitiveClosureMethod(m.clone(), order);
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;
        }

        //实验四：环路消减法的对比实验
        private void button3_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            List<int> cy_weight = ThirdMethod.ThirdTechnique(m.clone());
            List<string> order = OptimalSet.GeneratedOrderbyWeight(cy_weight);

            textBox5.Text += "回路计数法：";
            textBox5.Text += DFA2RE.TransitiveClosureMethod(m.clone(), order);
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;
        }

        //主要功能
        //实验一：剪枝和穷举算法的对比实验，包括计算运行时间及消减操作数
        private void button4_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK); 
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            List<int> tr_weight = TimesDegree.StaticInDegree(m.clone());
            List<string> order = OptimalSet.GeneratedOrderbyWeight(tr_weight);

            textBox5.Text += "流量法：";
            textBox5.Text += DFA2RE.TransitiveClosureMethod(m.clone(), order);
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;
        }

        //辅助功能：用于输入log文件
        private void button5_Click(object sender, EventArgs e)
        {

            OpenFileDialog filename = new OpenFileDialog();

            filename.InitialDirectory = Directory.GetCurrentDirectory();
            if (filename.ShowDialog() == DialogResult.OK)
            {
                string path = filename.FileName.ToString();
                //textBox2.Text = path;
            }
        }
        //实验三：正确率对比实验
        private void button6_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            textBox5.Text += "权重法：";
            textBox5.Text += DelgadoandMorais.DynamicDMandTCMsim(m.clone());
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog filename = new OpenFileDialog();

            filename.InitialDirectory = Directory.GetCurrentDirectory();
            if (filename.ShowDialog() == DialogResult.OK)
            {
                string path = filename.FileName.ToString();
                textBox3.Text = path;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            string dec_code = automata_code;

            DateTime beforDT = DateTime.Now;

            StateMachine temp_m = Construct.GenerateAutomata(states, ref automata_code, true);
            StateMachine m = Algorithm.DeleteRedundancy(temp_m.clone());

            List<string> order = SecondMethod.DynamicEliminationMethod(m.clone(), "constant");

            textBox5.Text += "改进权重法：";
            textBox5.Text += DFA2RE.TransitiveClosureMethod(m.clone(), order);
            textBox5.Text += "\r\n";

            DateTime afterDT = DateTime.Now;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string code = textBox3.Text;
            int states;
            string automata_code;
            try
            {
                states = Convert.ToInt32(code.Split('_')[0]) - 2;
                automata_code = code.Split('_')[1];
            }
            catch
            {
                MessageBox.Show("路径输入错误", "错误", MessageBoxButtons.OK);
                return;
            }

            bool flag = false;
            StateMachine s = new StateMachine();
            string bin_code = Construct.CodeConvert(states, 10, 2, automata_code);

            Construct.Adjacency(states, bin_code, ref flag, ref s);
            if (!flag) { MessageBox.Show("该图的初态和终态不可达", "错误", MessageBoxButtons.OK); return; }

            Algorithm.GenerateGraph(states, s, textBox3.Text);

            MessageBox.Show("自动机图生成完毕", "通知", MessageBoxButtons.OK);
        }
    }

    class TextBoxWriter : TextWriter
    {
        TextBox textBox;
        delegate void WriteFunc(string value);
        WriteFunc write;
        WriteFunc writeLine;

        public TextBoxWriter(TextBox textBox)
        {
            this.textBox = textBox;
            write = Write;
            writeLine = WriteLine;
        }

        // 使用UTF-16避免不必要的编码转换
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        // 最低限度需要重写的方法
        public override void Write(string value)
        {
            if (textBox.InvokeRequired)
                textBox.BeginInvoke(write, value);
            else
                textBox.AppendText(value);
        }

        // 为提高效率直接处理一行的输出
        public override void WriteLine(string value)
        {
            if (textBox.InvokeRequired)
                textBox.BeginInvoke(writeLine, value);
            else
            {
                textBox.AppendText(value);
                textBox.AppendText(this.NewLine);
            }
        }
    }

}
