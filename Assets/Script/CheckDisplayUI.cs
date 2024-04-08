using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ver2
{
    public class CheckDisplayUI : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public MainUI mainUI;

        [SerializeField] GameObject checkPointGo;
        [SerializeField] RectTransform selfRect;


        CheckPointItem[] checkPointItems;
        public int checkNum;

        // Start is called before the first frame update
        void Awake()
        {
            //初始化对象池
            checkPointItems = new CheckPointItem[20];
            for (int i = 0; i < 20; i++)
            {
                GameObject temp = Instantiate(checkPointGo, selfRect);
                checkPointItems[i] = temp.GetComponent<CheckPointItem>();
                checkPointItems[i].checkDisplay = this;
                temp.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //将容器中的check信息转换为数据
        public List<CheckPoint> CheckToInfo()
        {
            List<CheckPoint> res = new List<CheckPoint>();
            if (checkNum <= 0) return res;

            //根据checkNum值对子物体进行扫描，并储存其信息
            for (int i = selfRect.childCount - checkNum; i < selfRect.childCount; i++)
            {
                CheckPointItem temp = selfRect.GetChild(i).gameObject.GetComponent<CheckPointItem>();
                res.Add(new CheckPoint(temp.pos, temp.num, temp.startTime, temp.endTime));
            }
            return res;
        }

        //提取歌词句子信息中的check信息
        public void InfoToCheck(Sentence sentenceInfo)
        {
            ClearCheck();
            if (sentenceInfo.checkPoints.Count == 0) return;
            foreach (CheckPoint point in sentenceInfo.checkPoints)
            {
                selfRect.GetChild(0).gameObject.GetComponent<CheckPointItem>().Init(point.pos, point.num, point.startTime, point.endTime);
            }
        }

        //激活对应索引的check
        public void SetHasCheck(int index)
        {
            selfRect.GetChild(selfRect.childCount - checkNum + index).GetComponent<CheckPointItem>().SetChecked(true);
        }

        //激活该句歌词除最后一个check之外的check，用于进度条后退
        public void AllSetHasCheck()
        {
            for (int i = selfRect.childCount - checkNum; i < selfRect.childCount - 1; i++)
            {
                selfRect.GetChild(i).GetComponent<CheckPointItem>().SetChecked(true);
            }
        }

        //清除面板上所有check
        public void ClearCheck()
        {
            for (int i = 0; i < 20; i++)
            {
                checkPointItems[i].Clear();
            }
            checkNum = 0;
        }

        //左键点击面板 添加check
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Screen.width == 1920)
                {
                    selfRect.GetChild(0).gameObject.GetComponent<CheckPointItem>().Init((int)Input.mousePosition.x - 960, 1, 0, 0);
                }
                else
                {
                    selfRect.GetChild(0).gameObject.GetComponent<CheckPointItem>().Init((int)(Input.mousePosition.x / Screen.width * 1920) - 960, 1, 0, 0);
                }
            }
        }
    }

}
