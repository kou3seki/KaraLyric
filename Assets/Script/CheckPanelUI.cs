using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckPanelUI : MonoBehaviour, IScrollHandler
{
    public int itemHeight;
    [SerializeField] RectTransform itemContainer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject sentenceGo;
    [SerializeField] Button extractBtn;
    [SerializeField] Button checkImport;
    [SerializeField] Button checkSave;
    [SerializeField] Button startBtn;
    [SerializeField] Button stopBtn;
    [SerializeField] Button pauseBtn;
    [SerializeField] Button goBackBtn;
    [SerializeField] Button lyricPlayBtn;
    [SerializeField] Text logText;
    [SerializeField] GameObject checkDisplayGo;
    CheckDisplayUI checkDisplayUI;
    [SerializeField] GameObject lyricPlayGo;
    LyricPlayUI lyricPlayUI;
    [SerializeField] GameObject phoneticDisplayGo;
    PhoneticDisplayUI phoneticDisplayUI;
    [SerializeField] InputField lyricInterval;
    int interval;

    public SentenceItem[] sentences;
    public List<Sentence> sentenceInfos;

    //check模式控制变量
    public int concentrate;
    int startIndex;
    int sentenceNum;
    bool isPlaying;
    int checkPointIndex;
    int curMaxCheckPointIndex;
    int pointNum;
    int curMaxPointNum;

    // Start is called before the first frame update
    void Start()
    {
        checkDisplayUI = checkDisplayGo.GetComponent<CheckDisplayUI>();
        lyricPlayUI = lyricPlayGo.GetComponent<LyricPlayUI>();
        phoneticDisplayUI = phoneticDisplayGo.GetComponent<PhoneticDisplayUI>();

        sentences = new SentenceItem[200];
        for (int i = 0; i < 200; i++)
        {
            GameObject temp = Instantiate(sentenceGo, itemContainer);
            sentences[i] = temp.GetComponent<SentenceItem>();
            temp.SetActive(false);
        }

        extractBtn.onClick.AddListener(ExtractLyric);
        checkImport.onClick.AddListener(CheckImport);
        checkSave.onClick.AddListener(CheckSave);
        startBtn.onClick.AddListener(StartCheck);
        stopBtn.onClick.AddListener(StopCheck);
        pauseBtn.onClick.AddListener(PauseCheck);
        goBackBtn.onClick.AddListener(GoBack);
        lyricPlayBtn.onClick.AddListener(LyricPlay);

        CheckImport();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && isPlaying)
        {
            AddCheckFinishTime();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isPlaying)
        {
            AddCheckStartTime();
        }

        if (Input.GetKeyUp(KeyCode.V) && isPlaying)
        {
            GoBack();
        }

        if (Input.GetKeyUp(KeyCode.Z) && !isPlaying)
        {
            StartCheck();
        }

        if (Input.GetKeyUp(KeyCode.X) && isPlaying)
        {
            StopCheck();
        }

        if (Input.GetKeyUp(KeyCode.C) && isPlaying)
        {
            PauseCheck();
        }
    }

    public void LyricPlay()
    {
        lyricPlayUI.sentenceInfos = sentenceInfos.ToArray();
        lyricPlayGo.SetActive(true);
        lyricPlayUI.StartPlay();
        gameObject.SetActive(false);
    }

    public void StartCheck()
    {
        audioSource.Play();
        isPlaying = true;
        concentrate = 0;
        checkDisplayUI.InfoToCheck(sentenceInfos[concentrate]);
        phoneticDisplayUI.InfoToPhonetic(sentenceInfos[concentrate]);
        itemContainer.anchoredPosition = new Vector2(-960, itemHeight * concentrate);

        checkPointIndex = 0;
        pointNum = 0;
        curMaxCheckPointIndex = sentenceInfos[concentrate].checkPoints.Count;
        if(sentenceInfos[concentrate].checkPoints.Count != 0)
        {
            curMaxPointNum = sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
        }
    }

    public void StopCheck()
    {
        audioSource.time = 0;
        audioSource.Stop();
        isPlaying = false;
    }

    public void PauseCheck()
    {
        audioSource.Pause();
        isPlaying = false;
    }

    public void GoBack()
    {
        if (concentrate == 0) return;
        PreSentence();
        audioSource.time = sentenceInfos[concentrate].checkPoints[0].startTime / 1000f;
        checkDisplayUI.AllSetHasCheck();
        curMaxCheckPointIndex = sentenceInfos[concentrate].checkPoints.Count;
        checkPointIndex = curMaxCheckPointIndex - 1;
        pointNum = 0;
        curMaxPointNum = sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
    }

    //将所以歌词信息，转换为字符串导出保存
    public void CheckSave()
    {
        StreamWriter writer = new StreamWriter("D:/KaraLyric/ingredient/Check.txt");
        foreach (Sentence sentence in sentenceInfos)
        {
            writer.WriteLine(sentence.ToString());
        }
        writer.Flush();
        writer.Close();
    }

    //导入保存的check信息
    public void CheckImport()
    {
        StreamReader reader = new StreamReader("D:/KaraLyric/ingredient/Check.txt");
        sentenceInfos = new List<Sentence>();
        bool readFlag = true;
        string temp;
        sentenceNum = 0;
        concentrate = 0;
        itemContainer.anchoredPosition = new Vector2(-960, 0);
        for (int i = 0; i < 200; i++)
        {
            if (!readFlag)
            {
                sentences[i].ClearContent();
                continue;
            }

            temp = reader.ReadLine();
            if (temp == null)
            {
                sentences[i].ClearContent();
                readFlag = false;
            }
            else
            {
                Sentence tempInfo = new Sentence(temp);
                sentences[i].SetContent(tempInfo);
                sentenceInfos.Add(tempInfo);
                sentenceNum++;
            }
        }

        checkDisplayUI.InfoToCheck(sentenceInfos[0]);
        phoneticDisplayUI.InfoToPhonetic(sentenceInfos[0]);
        itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight * (sentenceNum + startIndex));
        reader.Close();
    }

    //从对应路径的文档中，每隔一定句数导入歌词
    public void ExtractLyric()
    {
        StreamReader reader;
        if (!int.TryParse(lyricInterval.text, out interval))
        {
            interval = 4;
        }

        try
        {
            reader = new StreamReader("D:/KaraLyric/ingredient/lyric.txt");
        }
        catch
        {
            logText.text = "未在对应路径(D:/KaraLyric/ingredient/lyric.txt)找到歌词文件";
            return;
        }

        sentenceInfos = new List<Sentence>();
        string temp;
        sentenceNum = 0;
        concentrate = 0;
        bool readFlag = true;
        for (int i = 0; i < 200; i++)
        {
            if (!readFlag)
            {
                sentences[i].ClearContent();
                continue;
            }

            temp = reader.ReadLine();
            for (int j = 0; j < interval - 1; j++)
            {
                reader.ReadLine();
            }

            if (temp == null)
            {
                sentences[i].ClearContent();
                readFlag = false;
            }
            else
            {
                string[] temp1 = temp.Split('|');
                Sentence tempInfo = new Sentence(temp1[0]);
                sentences[i].SetContent(tempInfo);
                sentenceInfos.Add(tempInfo);
                sentenceNum++;
            }
        }
        reader.Close();

        itemContainer.anchoredPosition = new Vector2(-960, 0);
        itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight * (sentenceNum + startIndex));
    }

    //滚轮事件
    public void OnScroll(PointerEventData eventData)
    {
        if(eventData.scrollDelta.y > 0 && concentrate > 0)
        {
            Sentence temp = sentenceInfos[concentrate];
            temp.checkPoints = checkDisplayUI.CheckToInfo();
            temp.phonetics = phoneticDisplayUI.PhoneticToInfo();
            sentenceInfos[concentrate] = temp;
            PreSentence();
        }

        if (eventData.scrollDelta.y < 0 && concentrate < sentenceNum - 1)
        {
            Sentence temp = sentenceInfos[concentrate];
            temp.checkPoints = checkDisplayUI.CheckToInfo();
            temp.phonetics = phoneticDisplayUI.PhoneticToInfo();
            sentenceInfos[concentrate] = temp;
            NextSentence();
        }
    }

    //高亮移动到上一句歌词
    public void NextSentence()
    {
        concentrate++;
        if(concentrate >= sentenceNum)
        {
            concentrate = sentenceNum - 1;
            return;
        }
        checkDisplayUI.InfoToCheck(sentenceInfos[concentrate]);
        phoneticDisplayUI.InfoToPhonetic(sentenceInfos[concentrate]);
        itemContainer.anchoredPosition = new Vector2(-960, itemHeight * concentrate);
    }

    //高亮移动到下一句歌词
    public void PreSentence()
    {
        concentrate--;
        checkDisplayUI.InfoToCheck(sentenceInfos[concentrate]);
        phoneticDisplayUI.InfoToPhonetic(sentenceInfos[concentrate]);
        itemContainer.anchoredPosition = new Vector2(-960, itemHeight * concentrate);
    }

    //check按键，按下时记录开始时间
    public void AddCheckStartTime()
    {
        if (pointNum != 0) return;

        Sentence temp1 = sentenceInfos[concentrate];
        CheckPoint temp2 = temp1.checkPoints[checkPointIndex];
        temp2.startTime = (int)(audioSource.time * 1000);
        temp1.checkPoints[checkPointIndex] = temp2;
        sentenceInfos[concentrate] = temp1;
    }

    //check按键，抬起时记录结束时间
    public void AddCheckFinishTime()
    {
        pointNum++;
        if (pointNum < curMaxPointNum) return;

        pointNum = 0;
        Sentence temp1 = sentenceInfos[concentrate];
        CheckPoint temp2 = temp1.checkPoints[checkPointIndex];
        temp2.endTime = (int)(audioSource.time * 1000);
        temp1.checkPoints[checkPointIndex] = temp2;
        sentenceInfos[concentrate] = temp1;
        checkDisplayUI.SetHasCheck(checkPointIndex);
        checkPointIndex++;
        if (checkPointIndex < curMaxCheckPointIndex)
        {
            curMaxPointNum = sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
        }
        else
        {
            checkPointIndex = 0;
            NextSentence();
            curMaxCheckPointIndex = sentenceInfos[concentrate].checkPoints.Count;
            curMaxPointNum = sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
        }
    }
}
