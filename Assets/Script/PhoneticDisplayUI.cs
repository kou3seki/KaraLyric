using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ver2
{
    public class PhoneticDisplayUI : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public MainUI mainUI;

        [SerializeField] GameObject phoneticGo;
        [SerializeField] RectTransform selfRect;
        public InputField inputField;

        PhoneticItem[] phoneticItems;
        [HideInInspector] PhoneticItem currentInput;

        // Start is called before the first frame update
        void Awake()
        {
            phoneticItems = new PhoneticItem[20];
            for (int i = 0; i < 20; i++)
            {
                GameObject temp = Instantiate(phoneticGo, selfRect);
                phoneticItems[i] = temp.GetComponent<PhoneticItem>();
                phoneticItems[i].phoneticDisplayUI = this;
                temp.SetActive(false);
            }

            inputField.onValueChanged.AddListener(OnInput);
            inputField.onEndEdit.AddListener(EndInput);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputField.DeactivateInputField();
            }
        }

        public List<Phonetic> PhoneticToInfo()
        {
            List<Phonetic> res = new List<Phonetic>();

            for (int i = selfRect.childCount - 1; i >= 0; i--)
            {
                GameObject tempGo = selfRect.GetChild(i).gameObject;
                if (!tempGo.activeInHierarchy) break;
                PhoneticItem temp = tempGo.GetComponent<PhoneticItem>();

                int insertIndex = 0;
                for (int j = 0; j < res.Count; j++)
                {
                    if (res[j].pos < temp.pos) insertIndex++;
                    else break;
                }
                res.Insert(insertIndex, new Phonetic(temp.text.text, temp.pos));
            }

            return res;
        }

        public void InfoToPhonetic(Sentence sentenceInfo)
        {
            ClearPhonetic();
            foreach (Phonetic phonetic in sentenceInfo.phonetics)
            {
                selfRect.GetChild(0).gameObject.GetComponent<PhoneticItem>().Init(phonetic.pos, phonetic.content);
            }
        }

        public void ClearPhonetic()
        {
            for (int i = 0; i < 20; i++)
            {
                phoneticItems[i].Clear();
            }
        }

        public void OnInput(string input)
        {
            if (!mainUI.isCtrlKey && currentInput != null)
            {
                currentInput.text.text = input;
            }
        }

        public void EndInput(string input)
        {
            if (currentInput != null)
            {
                currentInput.SetIsActive(false);
                currentInput = null;
            }
        }

        //对第index个注音进行输入
        public void SetInput(int index)
        {
            SetCurrentInput(null);
            int count = mainUI.sentenceInfos[mainUI.concentrate].phonetics.Count;
            if (count <= 0) return;
            if (index < 0) index =  0;
            if (index > count - 1) index = count - 1;
            SetCurrentInput(selfRect.GetChild(20 - count + index).gameObject.GetComponent<PhoneticItem>());
        }

        public void SetCurrentInput(PhoneticItem phoneticItem)
        {
            if(currentInput != null) currentInput.SetIsActive(false);
            if (phoneticItem == null || mainUI.playControlUI.isPlaying) return;
            currentInput = phoneticItem;
            currentInput.SetIsActive(true);
            inputField.text = "";
            inputField.ActivateInputField();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && !mainUI.playControlUI.isPlaying)
            {
                PhoneticItem temp = selfRect.GetChild(0).gameObject.GetComponent<PhoneticItem>();
                if (Screen.width == 1920)
                {
                    temp.Init((int)Input.mousePosition.x - 960);
                }
                else
                {
                    temp.Init((int)(Input.mousePosition.x / Screen.width * 1920) - 960);
                }

                SetCurrentInput(temp);
            }
        }
    }
}
