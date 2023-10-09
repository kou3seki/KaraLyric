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

    //进度条控制变量
    List<CheckPoint> checkPoints;
    int curSentenceIndex;
    int nextCheckIndex;
    int curMaxCheckIndex;
    float nextCheckStartTime;
    float nextCheckEndTime;
    float nextCheckPos;
    bool barMoving;
    bool[] needDisappear;

    //歌词顺序显示控制变量
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
        //点击任意键退出播放
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
        //进度条初始化
        texts[0].text = "";
        texts[1].text = "";
        bars[0].anchoredPosition = new Vector2(-1000, 0);
        bars[1].anchoredPosition = new Vector2(-1000, 0);

        //进度条控制变量初始化
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

        //歌词顺序显示控制变量初始化
        nextSentenceIndex = 0;
        nextSentenceTime = sentenceInfos[nextSentenceIndex].checkPoints[0].startTime / 1000f - 1;
        textIndex = 0;
        barIndex = 0;
        curTextWidth = texts[barIndex].GetComponent<RectTransform>();

        //开始播放
        audioSource.Play();
        isPlaying = true;
    }

    //根据当前的checkPoints和nextCheckIndex确定下一个check的结束时间
    public void SetNextCheck()
    {
        nextCheckStartTime = checkPoints[nextCheckIndex].startTime / 1000f - 0.3f;
        if (nextCheckIndex + 1 < curMaxCheckIndex)
        {
            //若不为本句最后一个check，则结束时间为下一个check的开始时间
            nextCheckEndTime = checkPoints[nextCheckIndex + 1].startTime / 1000f - 0.3f;
        }
        else
        {
            //若为本句最后一个check，则结束时间为该check结束
            nextCheckEndTime = checkPoints[nextCheckIndex].endTime / 1000f - 0.3f;
        }
        nextCheckPos = checkPoints[nextCheckIndex].pos;
    }

    public void LyricDisplay()
    {
        //如果不再播放状态或歌词已遍历完毕，则不进行操作
        if (!isPlaying || curSentenceIndex >= sentenceInfos.Length)
        {
            return;
        }

        //如果歌曲播放到下一句歌词开始时间，则显示下一句歌词
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

        //如果到达下一个check的开始时间，则初始化进度条移动
        if(nextCheckStartTime <= audioSource.time && !barMoving)
        {
            Tween temp = bars[barIndex].DOAnchorPos(new Vector2(nextCheckPos * 1.33f - 1000 + curTextWidth.rect.width/2, 0), nextCheckEndTime - nextCheckStartTime);
            temp.OnComplete(OnCheckEnd);
            barMoving = true;
            nextCheckIndex++;

            //如果到达每句歌词末尾，则需延时清空歌词，并将curSentenceIndex移至下一句歌词
            if (nextCheckIndex >= curMaxCheckIndex)
            {
                needDisappear[barIndex] = true;
                curSentenceIndex++;

                //如果到达歌曲末尾，则停止播放
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

                //载入下一句歌词的check信息
                nextCheckIndex = 0;
                barIndex = 1 - barIndex; 
                curTextWidth = texts[barIndex].GetComponent<RectTransform>();
                checkPoints = sentenceInfos[curSentenceIndex].checkPoints;
                curMaxCheckIndex = checkPoints.Count;
            }
            SetNextCheck();
        }
    }

    //进度条在每一个check结束后的回调
    public void OnCheckEnd()
    {
        barMoving = false;

        //如果该句歌词需要消失，此时当前歌词一定在另一个分区
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
