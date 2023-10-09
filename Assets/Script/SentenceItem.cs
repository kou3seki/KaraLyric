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

//����check�����Ϣ
//pos��check���ڸ���еĿռ�λ�ã����ڽ�������λ
//num��check�㷢�����������ڼ�¼����õ���Ҫ���û�����
//startTime��endTime����checkʱ��check�����µ�ʱ���̧���ʱ��
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

//���ע�������Ϣ
//content��ע������
//pos��ע���ռ�λ��
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

//���ÿһ�����Ϣ
//content����ʵ��ı�����
//checkPoints���þ��ʵ�check�㼯��
//phonetics����ʵĸ���ע��
//color����ʵĽ�������ɫ
//pos����ʵ�λ�ò���
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

        //�����ɫ��ȡ
        if (temp1.Length < 2) return;
        ColorUtility.TryParseHtmlString("#" + temp1[1], out color);

        //���check���ȡ
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

        //���ע����ȡ
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