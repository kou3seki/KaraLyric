using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ver2;

namespace ver2
{
    public class PlayControlUI : MonoBehaviour
    {
        [HideInInspector]public MainUI mainUI;

        [SerializeField] AudioSource audioSource;

        [SerializeField] Button startBtn;
        [SerializeField] Button stopBtn;
        [SerializeField] Button goBackBtn;
        [SerializeField] Button lyricPlayBtn;
        [SerializeField] Button autoPlaceCheckBtn;

        [SerializeField] GameObject lyricPlayGo;
        LyricPlayUI lyricPlayUI;

        public bool isPlaying;
        int checkPointIndex;
        int curMaxCheckPointIndex;
        int pointNum;
        int curMaxPointNum;

        [SerializeField] InputField musicNameIF;
        [SerializeField] InputField vocalIF;
        [SerializeField] InputField albumIF;
        string musicName;
        string vocal;
        string album;

        // Start is called before the first frame update
        void Start()
        {
            lyricPlayUI = lyricPlayGo.GetComponent<LyricPlayUI>();
            lyricPlayUI.mainUI = mainUI;

            startBtn.onClick.AddListener(StartCheck);
            stopBtn.onClick.AddListener(StopCheck);
            goBackBtn.onClick.AddListener(GoBack);
            lyricPlayBtn.onClick.AddListener(LyricPlay);
            autoPlaceCheckBtn.onClick.AddListener(mainUI.AutoPlaceCheck);

            musicNameIF.onEndEdit.AddListener(delegate { musicName = musicNameIF.text; });
            vocalIF.onEndEdit.AddListener(delegate { vocal = vocalIF.text; });
            albumIF.onEndEdit.AddListener(delegate { album = albumIF.text; });
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.C) && isPlaying)
            {
                GoBack();
            }

            if (Input.GetKeyUp(KeyCode.A) && !isPlaying && mainUI.isCtrlKey)
            {
                StartCheck();
            }

            if (Input.GetKeyUp(KeyCode.X) && isPlaying)
            {
                StopCheck();
            }

            if (Input.GetKeyUp(KeyCode.D) && !isPlaying && mainUI.isCtrlKey)
            {
                LyricPlay();
            }

            if (Input.GetKeyUp(KeyCode.Space) && isPlaying)
            {
                AddCheckFinishTime();
            }

            if (Input.GetKeyDown(KeyCode.Space) && isPlaying)
            {
                AddCheckStartTime();
            }
        }

        void LyricPlay()
        {
            lyricPlayUI.sentenceInfos = mainUI.sentenceInfos.ToArray();
            lyricPlayUI.SetMusicInfo(musicName, vocal, album);
            lyricPlayGo.SetActive(true);
            mainUI.gameObject.SetActive(false);
            lyricPlayUI.StartPlay();
        }

        void StartCheck()
        {
            isPlaying = true;
            mainUI.phoneticDisplayUI.inputField.DeactivateInputField();
            audioSource.Play();
            int concentrate = 0;
            while (mainUI.sentenceInfos[concentrate].isChecked == 1 && concentrate < mainUI.sentenceInfos.Count - 1)
            {
                concentrate++;
            }


            //跳转到已经check过的歌词句的上一句
            if (concentrate > 0)
            {
                concentrate --;
                GoToOtherSentence(true, concentrate, true);
            }
            else
            {
                GoToOtherSentence(false, concentrate, true);
            }
        }

        void StopCheck()
        {
            audioSource.time = 0;
            audioSource.Stop();
            isPlaying = false;
        }

        void GoBack()
        {
            int concentrate = mainUI.concentrate;
            if (concentrate == 0)
            {
                GoToOtherSentence(false, 0, false);
                SetHasChecked(false, 0);
            }
            else
            {
                GoToOtherSentence(true, concentrate - 1, false);
                SetHasChecked(false, concentrate - 1);
            }
        }

        //进度标识，按下时记录开始时间
        void AddCheckStartTime()
        {
            if (pointNum != 0 || !isPlaying) return;

            int concentrate = mainUI.concentrate;
            Sentence temp1 = mainUI.sentenceInfos[concentrate];
            CheckPoint temp2 = temp1.checkPoints[checkPointIndex];
            temp2.startTime = (int)(audioSource.time * 1000);
            temp1.checkPoints[checkPointIndex] = temp2;
            mainUI.sentenceInfos[concentrate] = temp1;
        }

        //进度标识，抬起时记录结束时间
        void AddCheckFinishTime()
        {
            pointNum++;
            if (pointNum < curMaxPointNum || !isPlaying) return;

            pointNum = 0;
            int concentrate = mainUI.concentrate;
            Sentence temp1 = mainUI.sentenceInfos[concentrate];
            CheckPoint temp2 = temp1.checkPoints[checkPointIndex];
            temp2.endTime = (int)(audioSource.time * 1000);
            temp1.checkPoints[checkPointIndex] = temp2;
            mainUI.sentenceInfos[concentrate] = temp1;
            mainUI.checkDisplayUI.SetHasCheck(checkPointIndex);
            checkPointIndex++;
            if (checkPointIndex < curMaxCheckPointIndex)
            {
                curMaxPointNum = mainUI.sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
            }
            else
            {
                SetHasChecked(true, concentrate);
                concentrate++;
                if (concentrate >= mainUI.sentenceInfos.Count )
                {
                    mainUI.fileOperationUI.CheckSave();
                    return;
                }
                GoToOtherSentence(false, concentrate, false);
            }
        }

        //跳转到指定歌词，可设置是否跳转到最后一个进度标识
        void GoToOtherSentence(bool hasChecked, int concentrate, bool needSavePos)
        {
            if(needSavePos) mainUI.SaveCheckAndPhoneticPos();
            mainUI.RefreshHighlight(concentrate);
            if (hasChecked)
            {
                audioSource.time = mainUI.sentenceInfos[concentrate].checkPoints[0].startTime / 1000f;
                mainUI.checkDisplayUI.AllSetHasCheck();
                checkPointIndex = mainUI.sentenceInfos[concentrate].checkPoints.Count - 1;
            }
            else
            {
                checkPointIndex = 0;
            }

            curMaxCheckPointIndex = mainUI.sentenceInfos[concentrate].checkPoints.Count;
            curMaxPointNum = mainUI.sentenceInfos[concentrate].checkPoints[checkPointIndex].num;
            pointNum = 0;
        }

        //设置该句子是否已经完成进度标识
        void SetHasChecked(bool hasChecked, int concentrate)
        {
            Sentence temp = mainUI.sentenceInfos[concentrate];
            temp.isChecked = hasChecked?1:0;
            mainUI.sentenceInfos[concentrate] = temp;
        }

        public void SetMusicInfo(string musicName, string vocal, string album)
        {
            this.musicName = musicName;
            this.vocal = vocal;
            this.album = album;
            musicNameIF.text = musicName;
            vocalIF.text = vocal;
            albumIF.text = album;
        }

        public string[] GetMusicInfo()
        {
            string[] output = new string[3];
            output[0] = musicName;
            output[1] = vocal;
            output[2] = album;
            return output;
        }
    }
}