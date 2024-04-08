using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ver2
{
    public class ColorSelectUI : MonoBehaviour
    {
        [HideInInspector] public MainUI mainUI;
        [SerializeField] Camera mainCamera;

        [SerializeField] Button[] fontColors;
        [SerializeField] Button[] bgColors;
        [SerializeField] Button allSetColor;
        [SerializeField] Text logText;
        static Text log;

        void Awake()
        {
            log = logText;
        }

        // Start is called before the first frame update
        void Start()
        {
            fontColors[0].onClick.AddListener(delegate { SetFontColor(0); });
            fontColors[1].onClick.AddListener(delegate { SetFontColor(1); });
            fontColors[2].onClick.AddListener(delegate { SetFontColor(2); });
            fontColors[3].onClick.AddListener(delegate { SetFontColor(3); });
            fontColors[4].onClick.AddListener(delegate { SetFontColor(4); });
            fontColors[5].onClick.AddListener(delegate { SetFontColor(5); });
            fontColors[6].onClick.AddListener(delegate { SetFontColor(6); });
            fontColors[7].onClick.AddListener(delegate { SetFontColor(7); });

            bgColors[0].onClick.AddListener(delegate { SetBgColor(0); });
            bgColors[1].onClick.AddListener(delegate { SetBgColor(1); });
            bgColors[2].onClick.AddListener(delegate { SetBgColor(2); });
            bgColors[3].onClick.AddListener(delegate { SetBgColor(3); });
            bgColors[4].onClick.AddListener(delegate { SetBgColor(4); });
            bgColors[5].onClick.AddListener(delegate { SetBgColor(5); });
            bgColors[6].onClick.AddListener(delegate { SetBgColor(6); });
            bgColors[7].onClick.AddListener(delegate { SetBgColor(7); });

            allSetColor.onClick.AddListener(AllSetColor);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SetFontColor(int index)
        {
            int concentrate = mainUI.concentrate;
            Color tempColor = fontColors[index].GetComponent<Image>().color;
            Sentence temp = mainUI.sentenceInfos[concentrate];
            temp.color = tempColor;
            mainUI.sentenceInfos[concentrate] = temp;
            mainUI.sentences[concentrate].contentText.color = tempColor;
        }

        void SetBgColor(int index)
        {
            Color tempColor = fontColors[index].GetComponent<Image>().color;
            mainCamera.backgroundColor = tempColor;
        }

        //将所以歌词设置颜色
        void AllSetColor()
        {
            Color color = mainUI.sentenceInfos[mainUI.concentrate].color;
            int maxCount = mainUI.sentenceInfos.Count;
            for (int i = 0; i < maxCount; i++)
            {
                Sentence temp = mainUI.sentenceInfos[i];
                temp.color = color;
                mainUI.sentenceInfos[i] = temp;
                mainUI.sentences[i].contentText.color = color;
            }
        }

        public static void SetLogText(string content)
        {
            log.text = content;
        }
    }
}

