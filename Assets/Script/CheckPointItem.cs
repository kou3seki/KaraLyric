using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ver2
{
    public class CheckPointItem : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public CheckDisplayUI checkDisplay;
        [SerializeField] Text text;
        [SerializeField] GameObject hasChecked;

        //check点的信息
        [HideInInspector] public int num;
        [HideInInspector] public int pos;
        [HideInInspector] public int startTime;
        [HideInInspector] public int endTime;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //激活对象池中的check点gameObject
        public void Init(int pos, int num, int startTime, int endTime)
        {
            gameObject.SetActive(true);
            SetNum(num);
            SetPos(pos);
            this.startTime = startTime;
            this.endTime = endTime;
            transform.SetAsLastSibling();
            checkDisplay.checkNum++;
            SetChecked(false);
        }


        public void Clear()
        {
            gameObject.SetActive(false);
            SetNum(1);
            SetPos(0);
            startTime = 0;
            endTime = 0;
            transform.SetAsFirstSibling();
            checkDisplay.checkNum--;
            SetChecked(false);
        }

        public void SetNum(int num)
        {
            this.num = num;
            text.text = num.ToString();
        }

        public void SetPos(int pos)
        {
            this.pos = pos;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, 0);
        }

        public void SetChecked(bool flag)
        {
            hasChecked.SetActive(flag);
        }

        //左键点击增加check次数，右键点击减少check次数，check次数清零时消失
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                num--;
                if (num == 0)
                {
                    Clear();
                    return;
                }
                text.text = num.ToString();
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                num++;
                SetNum(num);
            }
        }
    }

}
