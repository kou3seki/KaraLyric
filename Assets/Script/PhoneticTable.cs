using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ver2
{
    public class PhoneticTable
    {
        private static Dictionary<string, PhoneticAndCheckNums> PhoneticConvert = LoadPhoneticTable();

        private PhoneticTable()
        {

        }

        private static Dictionary<string, PhoneticAndCheckNums> LoadPhoneticTable()
        {
            Dictionary<string, PhoneticAndCheckNums> output = new Dictionary<string, PhoneticAndCheckNums>();
            StreamReader reader = new StreamReader(Application.dataPath + "/Resources/Phonetic.txt");
            string temp1 = reader.ReadLine();
            while (temp1 != null)
            {
                string[] temp2 = temp1.Split(',');
                if (!output.ContainsKey(temp2[1])) output.Add(temp2[1], new PhoneticAndCheckNums(temp2[0], temp2[2]));
                temp1 = reader.ReadLine();
            }

            reader.Close();
            return output;
        }

        public static bool Get(string input, out PhoneticAndCheckNums output)
        {
            return PhoneticConvert.TryGetValue(input, out output);
        }

        public static void Add(string input)
        {
            string[] temp = input.Split('、');
            if(temp.Length < 3)
            {
                MainUI.SetLog("格式错误，添加失败");
                return;
            }
            if (PhoneticConvert.ContainsKey(temp[0])) PhoneticConvert[temp[0]] = new PhoneticAndCheckNums(temp[1], temp[2]);
            else PhoneticConvert.Add(temp[0], new PhoneticAndCheckNums(temp[1], temp[2]));
            MainUI.SetLog("添加成功");
        }

        public static void Save()
        {
            List<string> phoneticAndCheckNums = new List<string>();
            int length = 0;
            foreach(var pair in PhoneticConvert)
            {
                if (phoneticAndCheckNums.Count == 0) phoneticAndCheckNums.Add(pair.Value.phonetic + "," + pair.Key + "," + pair.Value.checkNum);
                else
                {
                    for (int i = 0; i <= length; i++)
                    {
                        string temp1 = pair.Value.phonetic + "," + pair.Key + "," + pair.Value.checkNum;
                        if (i == length)
                        {
                            phoneticAndCheckNums.Add(temp1);
                            break;
                        }
                        if (CompareItem(temp1, phoneticAndCheckNums[i]))
                        {
                            phoneticAndCheckNums.Insert(i, temp1);
                            break;
                        }
                    }
                }
                length++;
            }
            StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/Phonetic.txt");
            foreach(string item in phoneticAndCheckNums)
            {
                writer.WriteLine(item);
            }
            writer.Flush();
            writer.Close();
        }

        public static bool CompareItem(string input1, string input2)
        {
            int index = 0;
            while (input1.Length > index && input2.Length > index)
            {
                if (input1[index] == ',') return true;
                if (input1[index] > input2[index]) return false;
                if (input1[index] < input2[index]) return true;
                index++;
            }
            return false;
        }
    }

    public struct PhoneticAndCheckNums
    {
        public string phonetic;
        public string checkNum;

        public PhoneticAndCheckNums(string phonetic, string checkNum)
        {
            this.phonetic = phonetic;
            this.checkNum = checkNum;
        }
    }
}