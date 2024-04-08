using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ver2
{
    public class LyricPlayUI : MonoBehaviour
    {
        [HideInInspector] public MainUI mainUI;
        [SerializeField] AudioSource audioSource;
        public RectTransform poolContent;
        public GameObject phoneticTextGo;

        //歌词顺序显示控制参数
        int nextSentenceIndex;
        float nextSentenceTime;
        int playIndex;

        public Sentence[] sentenceInfos;
        [SerializeField] GameObject[] lyricPlayItemGos;
        LyricPlayItem[] lyricPlayItems;

        [SerializeField] Text musicName;
        [SerializeField] Text vocal;
        [SerializeField] Text album;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 40; i++)
            {
                GameObject temp = Instantiate(phoneticTextGo, poolContent);
                temp.GetComponent<PhoneticText>().lyricPlayUI = this;
                temp.SetActive(false);
            }

            lyricPlayItems = new LyricPlayItem[2];
            lyricPlayItems[0] = lyricPlayItemGos[0].GetComponent<LyricPlayItem>();
            lyricPlayItems[0].lyricPlayUI = this;
            lyricPlayItems[1] = lyricPlayItemGos[1].GetComponent<LyricPlayItem>();
            lyricPlayItems[1].lyricPlayUI = this;

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            //点击任意键退出播放
            if (Input.anyKeyDown)
            {
                FinishPlay("");
            }

            LyricDisplay();
        }

        public void SetMusicInfo(string musicName, string vocal, string album)
        {
            if (!musicName.Equals(""))
            {
                this.musicName.gameObject.SetActive(true);
                this.musicName.text = "曲名：" + musicName;
            }
            else this.musicName.gameObject.SetActive(false);
            if (!vocal.Equals(""))
            {
                this.musicName.gameObject.SetActive(true);
                this.vocal.text = "演唱：" + vocal;
            }
            else this.vocal.gameObject.SetActive(false);
            if (!album.Equals(""))
            {
                this.album.gameObject.SetActive(true);
                this.album.text = "专辑：" + album;
            }
            else this.album.gameObject.SetActive(false);
        }

        public void StartPlay()
        {
            if (sentenceInfos.Length == 0)
            {
                FinishPlay("当前无歌词");
                return;
            }

            //进度条初始化
            lyricPlayItems[0].Clear();
            lyricPlayItems[1].Clear();
            audioSource.time = 0;

            //歌词顺序显示控制参数初始化
            nextSentenceIndex = 0;
            while (sentenceInfos[nextSentenceIndex].checkPoints.Count == 0)
            {
                nextSentenceIndex++;
                if (nextSentenceIndex >= sentenceInfos.Length)
                {
                    FinishPlay("当前无任何歌词进度信息");
                    return;
                }
            }
            nextSentenceTime = sentenceInfos[nextSentenceIndex].checkPoints[0].startTime / 1000f - 1;

            //开始播放
            audioSource.Play();
        }

        public void FinishPlay(string log)
        {
            audioSource.Stop();
            MainUI.SetLog(log);
            mainUI.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        void LyricDisplay()
        {
            //如果歌曲播放到下一句歌词开始时间，则显示下一句歌词
            if (nextSentenceTime <= audioSource.time && nextSentenceIndex < sentenceInfos.Length)
            {
                lyricPlayItems[playIndex].SetLyric(sentenceInfos[nextSentenceIndex]);
                playIndex = 1 - playIndex;

                nextSentenceIndex++;
                while (nextSentenceIndex < sentenceInfos.Length && sentenceInfos[nextSentenceIndex].checkPoints.Count == 0)
                {
                    nextSentenceIndex++;
                }
                
                if (nextSentenceIndex < sentenceInfos.Length)
                {
                    nextSentenceTime = sentenceInfos[nextSentenceIndex].checkPoints[0].startTime / 1000f - 1;
                }
            }
        }
    }
}
