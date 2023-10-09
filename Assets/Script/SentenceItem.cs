using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class SentenceItem : MonoBehaviour
{
    public Text contentText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetContent(Sentence sentenceInfo)
    {
        if (sentenceInfo.content.Equals("")) return;

        contentText.text = sentenceInfo.content;
        gameObject.SetActive(true);
        contentText.color = sentenceInfo.color;
    }

    public void ClearContent()
    {
        gameObject.SetActive(false);
    }
}

//单个check点的信息
//pos，check点在歌词中的空间位置，用于进度条定位
//num，check点发音数量，用于记录到达该点需要的敲击次数
//startTime，endTime，打check时的check键按下的时间和抬起的时间
public struct CheckPoint
{
    public int pos;
    public int num;
    public int startTime;
    public int endTime;

    public CheckPoint(int pos, int num, int startTime, int endTime)
    {
        this.pos = pos;
        this.num = num;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public override string ToString()
    {
        StringBuilder res = new StringBuilder();
        res.Append(pos);
        res.Append("*");
        res.Append(num);
        res.Append("*");
        res.Append(startTime);
        res.Append("*");
        res.Append(endTime);
        return res.ToString();
    }
}

//歌词注音点的信息
//content，注音内容
//pos，注音空间位置
public struct Phonetic
{
    public string content;
    public int pos;

    public Phonetic(string content, int pos)
    {
        this.content = content;
        this.pos = pos;
    }

    public override string ToString()
    {
        return content + "*" + pos;
    }
}

//歌词每一句的信息
//content，歌词的文本内容
//checkPoints，该句歌词的check点集合
//phonetics，歌词的附加注释
//color，歌词的进度条颜色
//pos，歌词的位置补偿
public struct Sentence
{
    public string content;
    public List<CheckPoint> checkPoints;
    public List<Phonetic> phonetics;
    public Color color;

    public Sentence(string info)
    {
        string[] temp1 = info.Split('|');
        content = temp1[0];
        checkPoints = new List<CheckPoint>();
        phonetics = new List<Phonetic>();
        color = new Color(0.8679245f, 0.3194892f, 0.281666f);

        //歌词颜色读取
        if (temp1.Length < 2) return;
        ColorUtility.TryParseHtmlString("#" + temp1[1], out color);

        //歌词check点读取
        if (temp1.Length < 3) return;
        if (!temp1[2].Equals(""))
        {
            string[] temp2 = temp1[2].Split('/');
            foreach (string point in temp2)
            {
                string[] temp3 = point.Split('*');
                checkPoints.Add(new CheckPoint(int.Parse(temp3[0]), int.Parse(temp3[1]), int.Parse(temp3[2]), int.Parse(temp3[3])));
            }
        }

        //歌词注音读取
        if (temp1.Length < 4) return;
        if (!temp1[3].Equals(""))
        {
            string[] temp2 = temp1[3].Split('/');
            foreach (string phonetic in temp2)
            {
                string[] temp3 = phonetic.Split('*');
                phonetics.Add(new Phonetic(temp3[0], int.Parse(temp3[1]))) ;
            }
        }
    }

    public override string ToString()
    {
        StringBuilder res = new StringBuilder();
        res.Append(content);
        res.Append("|");
        res.Append(ColorUtility.ToHtmlStringRGB(color));
        res.Append("|");

        for (int i = 0; i < checkPoints.Count; i++)
        {
            res.Append(checkPoints[i].ToString());
            if(i != checkPoints.Count - 1) res.Append("/");
        }
        res.Append("|");

        for (int i = 0; i < phonetics.Count; i++)
        {
            res.Append(phonetics[i].ToString());
            if (i != phonetics.Count - 1) res.Append("/");
        }
        return res.ToString();
    }
}