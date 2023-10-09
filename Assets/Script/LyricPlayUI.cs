using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LyricPlayUI : MonoBehaviour
{
    [SerializeField] GameObject checkPanelUI;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Text[] texts;
    [SerializeField] RectTransform[] bars;
    [SerializeField] RectTransform[] phoneticGo;
    public RectTransform poolContent;
    RectTransform curTextWidth;

    bool isPlaying;

    //���������Ʊ���
    List<CheckPoint> checkPoints;
    int curSentenceIndex;
    int nextCheckIndex;
    int curMaxCheckIndex;
    float nextCheckStartTime;
    float nextCheckEndTime;
    float nextCheckPos;
    bool barMoving;
    bool[] needDisappear;

    //���˳����ʾ���Ʊ���
    int nextSentenceIndex;
    float nextSentenceTime;
    int textIndex;
    int barIndex;

    public Sentence[] sentenceInfos;

    PhoneticText[] phoneticTexts;
    public GameObject phoneticTextGo;

    // Start is called before the first frame update
    void Start()
    {
        phoneticTexts = new PhoneticText[20];
        for (int i = 0; i < 20; i++)
        {
            GameObject temp = Instantiate(phoneticTextGo, poolContent);
            phoneticTexts[i] = temp.GetComponent<PhoneticText>();
            phoneticTexts[i].lyricPlayUI = this;
            temp.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //���������˳�����
        if (Input.anyKeyDown)
        {
            audioSource.Stop();
            isPlaying = false;
            checkPanelUI.SetActive(true);
            gameObject.SetActive(false);

            int temp1 = phoneticGo[0].childCount;
            for (int j = 0; j < temp1; j++)
            {
                phoneticGo[0].GetChild(0).GetComponent<PhoneticText>().Clear();
            }
            temp1 = phoneticGo[1].childCount;
            for (int j = 0; j < temp1; j++)
            {
                phoneticGo[1].GetChild(0).GetComponent<PhoneticText>().Clear();
            }
        }

        LyricDisplay();
    }

    public void StartPlay()
    {
        //��������ʼ��
        texts[0].text = "";
        texts[1].text = "";
        bars[0].anchoredPosition = new Vector2(-1000, 0);
        bars[1].anchoredPosition = new Vector2(-1000, 0);

        //���������Ʊ�����ʼ��
        audioSource.time = 0;
        curSentenceIndex = 0;
        nextCheckIndex = 0;
        checkPoints = sentenceInfos[curSentenceIndex].checkPoints;
        curMaxCheckIndex = checkPoints.Count;
        SetNextCheck();
        barMoving = false;
        needDisappear = new bool[2];
        needDisappear[0] = false;
        needDisappear[1] = false;

        //���˳����ʾ���Ʊ�����ʼ��
        nextSentenceIndex = 0;
        nextSentenceTime = sentenceInfos[nextSentenceIndex].checkPoints[0].startTime / 1000f - 1;
        textIndex = 0;
        barIndex = 0;
        curTextWidth = texts[barIndex].GetComponent<RectTransform>();

        //��ʼ����
        audioSource.Play();
        isPlaying = true;
    }

    //���ݵ�ǰ��checkPoints��nextCheckIndexȷ����һ��check�Ľ���ʱ��
    public void SetNextCheck()
    {
        nextCheckStartTime = checkPoints[nextCheckIndex].startTime / 1000f - 0.3f;
        if (nextCheckIndex + 1 < curMaxCheckIndex)
        {
            //����Ϊ�������һ��check�������ʱ��Ϊ��һ��check�Ŀ�ʼʱ��
            nextCheckEndTime = checkPoints[nextCheckIndex + 1].startTime / 1000f - 0.3f;
        }
        else
        {
            //��Ϊ�������һ��check�������ʱ��Ϊ��check����
            nextCheckEndTime = checkPoints[nextCheckIndex].endTime / 1000f - 0.3f;
        }
        nextCheckPos = checkPoints[nextCheckIndex].pos;
    }

    public void LyricDisplay()
    {
        //������ٲ���״̬�����ѱ�����ϣ��򲻽��в���
        if (!isPlaying || curSentenceIndex >= sentenceInfos.Length)
        {
            return;
        }

        //����������ŵ���һ���ʿ�ʼʱ�䣬����ʾ��һ����
        if(nextSentenceTime <= audioSource.time && nextSentenceIndex < sentenceInfos.Length)
        {
            texts[textIndex].text = sentenceInfos[nextSentenceIndex].content;
            bars[textIndex].gameObject.GetComponent<Image>().color = sentenceInfos[nextSentenceIndex].color;

            foreach(Phonetic phonetic in sentenceInfos[nextSentenceIndex].phonetics)
            {
                PhoneticText temp = poolContent.GetChild(0).GetComponent<PhoneticText>();
                temp.Init(phonetic.content, phonetic.pos * 1.33f, sentenceInfos[nextSentenceIndex].color, phoneticGo[textIndex]);
            }

            nextSentenceIndex++;

            if (nextSentenceIndex < sentenceInfos.Length)
            {
                nextSentenceTime = sentenceInfos[nextSentenceIndex].checkPoints[0].startTime/1000f - 1;
                textIndex = 1 - textIndex;
            }
        }

        //���������һ��check�Ŀ�ʼʱ�䣬���ʼ���������ƶ�
        if(nextCheckStartTime <= audioSource.time && !barMoving)
        {
            Tween temp = bars[barIndex].DOAnchorPos(new Vector2(nextCheckPos * 1.33f - 1000 + curTextWidth.rect.width/2, 0), nextCheckEndTime - nextCheckStartTime);
            temp.OnComplete(OnCheckEnd);
            barMoving = true;
            nextCheckIndex++;

            //�������ÿ����ĩβ��������ʱ��ո�ʣ�����curSentenceIndex������һ����
            if (nextCheckIndex >= curMaxCheckIndex)
            {
                needDisappear[barIndex] = true;
                curSentenceIndex++;

                //����������ĩβ����ֹͣ����
                if (curSentenceIndex >= sentenceInfos.Length)
                {
                    isPlaying = false;
                    int temp1 = phoneticGo[0].childCount;
                    for (int j = 0; j < temp1; j++)
                    {
                        phoneticGo[0].GetChild(0).GetComponent<PhoneticText>().Clear();
                    }
                    temp1 = phoneticGo[1].childCount;
                    for (int j = 0; j < temp1; j++)
                    {
                        phoneticGo[1].GetChild(0).GetComponent<PhoneticText>().Clear();
                    }
                    return;
                }

                //������һ���ʵ�check��Ϣ
                nextCheckIndex = 0;
                barIndex = 1 - barIndex; 
                curTextWidth = texts[barIndex].GetComponent<RectTransform>();
                checkPoints = sentenceInfos[curSentenceIndex].checkPoints;
                curMaxCheckIndex = checkPoints.Count;
            }
            SetNextCheck();
        }
    }

    //��������ÿһ��check������Ļص�
    public void OnCheckEnd()
    {
        barMoving = false;

        //����þ�����Ҫ��ʧ����ʱ��ǰ���һ������һ������
        if (needDisappear[0])
        {
            Tween temp = transform.DOScale(new Vector3(1, 1, 1), 0.2f);
            temp.OnComplete(delegate
            {
                texts[0].text = "";
                bars[0].anchoredPosition = new Vector2(-1000, 0);

                int temp = phoneticGo[0].childCount;
                for (int i = 0; i < temp; i++)
                {
                    phoneticGo[0].GetChild(0).GetComponent<PhoneticText>().Clear();
                }
                needDisappear[0] = false;
            });
        }

        if (needDisappear[1])
        {
            Tween temp = transform.DOScale(new Vector3(1, 1, 1), 0.2f);
            temp.OnComplete(delegate
            {
                texts[1].text = "";
                bars[1].anchoredPosition = new Vector2(-1000, 0);

                int temp = phoneticGo[1].childCount;
                for (int i = 0; i < temp; i++)
                {
                    phoneticGo[1].GetChild(0).GetComponent<PhoneticText>().Clear();
                }
                needDisappear[1] = false;
            });
        }
    }
}
