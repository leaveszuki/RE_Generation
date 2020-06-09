using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZQStringRuleMining
{
    class Mode
    {
        public string mode; //1代表数字，2代表小写字母，3代表大写字母，4代表其他字符
        public int times;//这种模式在字符串数组里出现的次数
        public int[] num;//每种元素出现的次数
        public static int HaveMode(char c)
        {
            if (c >= 'a' && c <= 'z')
                return 2;
            else if (c >= 'A' && c <= 'Z')
                return 3;
            else if (c >= '0' && c <= '9')
                return 1;
            else
                return 4;

        }
        public Mode(string Mode,int Times,int[] Num)
        {
            mode = Mode;
            times = Times;
            num = Num;
        }
    }
    class ModeManage
    {
        List<Mode> ListMode = new List<Mode>();
        public void IsExist(Mode newMode)
        {
            bool IsExist=false;
            int i=0;
            for(i=0;i<ListMode.Count;i++)
                if (newMode.mode == ListMode[i].mode)
                {
                    IsExist = true;
                    break;
                }
            if (IsExist)
            {
                ListMode[i].times++;
                for (int j = 0; j < newMode.mode.Length; j++)
                    if (ListMode[i].num[j] < newMode.num[j])
                        ListMode[i].num[j] = newMode.num[j];
            }
            else
                ListMode.Add(newMode);
        }
        public string HaveTheBest()
        {
            int Max = 0;
            int Location=-1;
            for (int i = 0; i < ListMode.Count; i++)
            {
                if (ListMode[i].times > Max)
                {
                    Max = ListMode[i].times;
                    Location = i;
                }
            }                
            string Regular = "";
            if (Location != -1)
            {
                string Mode = ListMode[Location].mode;
                int[] num = ListMode[Location].num;

                for (int i = 0; i < Mode.Length; i++)
                {
                    if (Mode[i] == '1')
                        Regular += "[0,9]{1," + num[i].ToString() + "}";
                    if (Mode[i] == '2')
                        Regular += "[a,z]{1," + num[i].ToString() + "}";
                    if (Mode[i] == '3')
                        Regular += "[A,Z]{1," + num[i].ToString() + "}";
                }
            }
            return Regular;
        }
    }
}
