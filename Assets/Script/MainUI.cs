using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ver2
{
    public class MainUI : MonoBehaviour, IScrollHandler
    {
        public const int itemHeight = 90;
        public const int fontSize = 60;
        public const int halfFontSize = 30;

        [SerializeField] GameObject checkDisplayGo;
        [HideInInspector] public CheckDisplayUI checkDisplayUI;
        [SerializeField] GameObject phoneticDisplayGo;
        [HideInInspector] public PhoneticDisplayUI phoneticDisplayUI;
        [SerializeField] GameObject playControlGo;
        [HideInInspector] public PlayControlUI playControlUI;
        [SerializeField] GameObject fileOperationGo;
        [HideInInspector] public FileOperationUI fileOperationUI;
        [SerializeField] GameObject colorSelectGo;
        [HideInInspector] public ColorSelectUI colorSelectUI;


        [SerializeField] RectTransform itemContainer;
        [SerializeField] GameObject sentenceGo;
        [HideInInspector] public SentenceItem[] sentences;
        [HideInInspector] public List<Sentence> sentenceInfos;

        [HideInInspector] public int concentrate;
        float keyInterval;
        public bool isCtrlKey;
        float ctrlKeyCounter;

        private Dictionary<char, int> charWidth;
        private bool haveGetCharWidth;

        // Start is called before the first frame update
        void Awake()
        {
            checkDisplayUI = checkDisplayGo.GetComponent<CheckDisplayUI>();
            checkDisplayUI.mainUI = this;
            phoneticDisplayUI = phoneticDisplayGo.GetComponent<PhoneticDisplayUI>();
            phoneticDisplayUI.mainUI = this;
            playControlUI = playControlGo.GetComponent<PlayControlUI>();
            playControlUI.mainUI = this;
            fileOperationUI = fileOperationGo.GetComponent<FileOperationUI>();
            fileOperationUI.mainUI = this;
            colorSelectUI = colorSelectGo.GetComponent<ColorSelectUI>();
            colorSelectUI.mainUI = this;

            sentenceInfos = new List<Sentence>();
            sentences = new SentenceItem[200];
            for (int i = 0; i < 200; i++)
            {
                GameObject temp = Instantiate(sentenceGo, itemContainer);
                sentences[i] = temp.GetComponent<SentenceItem>();
                temp.SetActive(false);
            }

            GetCharWidth();
        }

        // Update is called once per frame
        void Update()
        {
            if(keyInterval >=0) keyInterval -= Time.deltaTime;

            if (Input.GetKey(KeyCode.UpArrow) && keyInterval < 0 && !playControlUI.isPlaying)
            {
                SaveCheckAndPhoneticPos();
                RefreshHighlight(concentrate - 1);
                keyInterval = 0.2f;
            }

            if (Input.GetKey(KeyCode.DownArrow) && keyInterval < 0 && !playControlUI.isPlaying)
            {
                SaveCheckAndPhoneticPos();
                RefreshHighlight(concentrate + 1);
                keyInterval = 0.2f;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCtrlKey = true;
                ctrlKeyCounter = 0.5f;
                phoneticDisplayUI.inputField.DeactivateInputField();
            }

            if (ctrlKeyCounter >= 0) ctrlKeyCounter -= Time.deltaTime;
            else isCtrlKey = false;
        }

        private void GetCharWidth()
        {
            if (haveGetCharWidth) return;
            charWidth = new Dictionary<char, int>
            {
                { ' ', 17 },{'\"', 28},{ '\'', 14 },{ ',', 17 },{'.', 17},{ '<', 35 },{'>', 35},{ '?', 37 },{ '!', 20 },{'+', 35},{'-', 20},{'*', 23},{'[', 20},{']', 20},{'(', 20},{')', 20},
                { '��', 60 },{'��', 30},{'��', 17},{'��', 30},{ '��', 17 },{ '��', 60 },{'��', 60},{ '��', 60 },{ '��', 60},{'��', 60},{ '��', 60 },{ '��', 60 },{'��', 60},{'��', 60},{'��', 60},
                { 'q', 37 },{ 'w', 47 },{'e', 33},{ 'r', 23 },{ 't', 20 },{'y', 33},{ 'u', 37 },{ 'i', 17 },{'o', 37},{ 'p', 37 },
                { 'a', 33 },{ 's', 33 },{'d', 37},{ 'f', 20 },{ 'g', 37 },{'h', 37},{ 'j', 17 },{ 'k', 33 },{'l', 17},
                { 'z', 30 },{ 'x', 33 },{'c', 33},{ 'v', 33 },{ 'b', 37 },{'n', 37},{ 'm', 53 },
                { 'Q', 47 },{ 'W', 57 },{'E', 40},{ 'R', 43 },{ 'T', 37 },{'Y', 40},{ 'U', 43 },{ 'I', 17 },{'O', 47},{ 'P', 40 },
                { 'A', 43 },{ 'S', 40 },{'D', 43},{ 'F', 37 },{ 'G', 47 },{'H', 43},{ 'J', 33 },{ 'K', 43 },{'L', 37},
                { 'Z', 37 },{ 'X', 40 },{'C', 43},{ 'V', 40 },{ 'B', 43 },{'N', 43},{ 'M', 50 },
                { '1', 33 },{ '2', 33 },{'3', 33},{ '4', 33 },{ '5', 33 },{'6', 33},{ '7', 33 },{ '8', 33 },{'9', 33},{ '0', 33 },
            };

            haveGetCharWidth = true;
        }

        //�Զ�ע��
        private bool AutoPhonetic(string input, out string phonetic, out string checkNums)
        {
            PhoneticAndCheckNums getResult;
            checkNums = "";
            phonetic = "";
            switch (input.Length)
            {
                case 0: return false;
                case 1://��������֣�ֱ�ӻ�ȡ����
                    if (PhoneticTable.Get(input, out getResult)) break;
                    return false;
                case 2://���˫���֣���ֱ�ӻ�ȡ���������޶����������������ֻ�ȡ
                    if (PhoneticTable.Get(input, out getResult)) break;
                    else
                    {
                        if (!PhoneticTable.Get(input[0].ToString(), out getResult)) return false;
                        PhoneticAndCheckNums temp = getResult;
                        if (!PhoneticTable.Get(input[1].ToString(), out getResult)) return false;
                        getResult.phonetic = temp.phonetic + getResult.phonetic;
                        getResult.checkNum = temp.checkNum + getResult.checkNum;
                        break;
                    }
                default://��������֣����������ַ��������޽����һ���ַ���������ָ���Ƶ���һ����
                    StringBuilder checkNumsSb = new StringBuilder();
                    StringBuilder phoneticSb = new StringBuilder();
                    int index = 0;
                    while (index < input.Length)
                    {
                        if (index == input.Length - 1)
                        {
                            if (PhoneticTable.Get(input[index].ToString(), out getResult))
                            {
                                phoneticSb.Append(getResult.phonetic);
                                checkNumsSb.Append(getResult.checkNum);
                                break;
                            }
                            return false;
                        }

                        if (PhoneticTable.Get(input.Substring(index, 2), out getResult))
                        {
                            phoneticSb.Append(getResult.phonetic);
                            checkNumsSb.Append(getResult.checkNum);
                            index += 2;
                        }
                        else
                        {
                            if (PhoneticTable.Get(input[index].ToString(), out getResult))
                            {
                                phoneticSb.Append(getResult.phonetic);
                                checkNumsSb.Append(getResult.checkNum);
                                index++;
                            }
                            else return false;
                        }
                    }
                    checkNums = checkNumsSb.ToString();
                    phonetic = phoneticSb.ToString();
                    return true;
            }
            checkNums = getResult.checkNum;
            phonetic = getResult.phonetic;
            return true;
        }

        //�ھ�����Ϣ�У����ע������Ϣ
        private void PlaceCheck(StringBuilder sb, int curPos, Sentence input)
        {
            int sbwidth = 30 * sb.Length;
            if (AutoPhonetic(sb.ToString(), out string phonetic, out string checkNum))
            {
                input.phonetics.Add(new Phonetic(phonetic, curPos - sbwidth));
                for (int i = 1; i <= sb.Length; i++)
                {
                    input.checkPoints.Add(new CheckPoint(curPos - 2 * sbwidth + i * fontSize, checkNum[i -1] - 48, 0, 0, 0));
                }
            }
            else
            {
                input.phonetics.Add(new Phonetic("", curPos - sbwidth));
                for (int i = 1; i <= sb.Length; i++)
                {
                    input.checkPoints.Add(new CheckPoint(curPos - 2 * sbwidth + i * fontSize, 1, 0, 0, 0));
                }
            }

            sb.Clear();
        }

        //�Զ���������ӽ��ȱ�ʶ��ע��λ�ã���ÿ���ֵĿ����ͬ����
        public void AutoPlaceCheck()
        {
            for (int i = 0; i < sentenceInfos.Count; i++)
            {
                Sentence temp = sentenceInfos[i];
                int curPos = - (int)sentences[i].contentText.preferredWidth/2;

                string content = temp.content;
                int count = content.Length;
                temp.checkPoints.Clear();
                temp.phonetics.Clear();

                StringBuilder sb = new StringBuilder();
                bool letterFLag = false;
                for (int j = 0; j < count; j++)
                {
                    //�������ĸ���Ϊ����
                    if (content[j] >= 'a' && content[j] <='z' || content[j] >= 'A' && content[j] <= 'Z')
                    {
                        letterFLag = true;
                        curPos += charWidth[content[j]];
                        if(j == count - 1) temp.checkPoints.Add(new CheckPoint(curPos, 1, 0, 0, 0));
                        continue;
                    }

                    //����ĸ�������Ҳ������ַ��򵥴������ˣ���¼һ�����ȵ�
                    if (letterFLag && content[j] != '-') temp.checkPoints.Add(new CheckPoint(curPos, 1, 0, 0, 0));
                    letterFLag = false;

                    //Ӣ���ж��������������ж�
                    //����Ǽ��������ע���б����һ�����ȵ�
                    if (content[j] >= 0x3040 && content[j] <= 0x309F || content[j] >= 0x30A0 && content[j] <= 0x30FF)
                    {
                        if (sb.Length != 0) PlaceCheck(sb, curPos, temp);
                        curPos += fontSize;
                        temp.checkPoints.Add(new CheckPoint(curPos, 1, 0, 0, 0));
                        continue;
                    }

                    //����������ַ������ָ��λ��ǰ��
                    if (charWidth.ContainsKey(content[j]))
                    {
                        if (sb.Length != 0) PlaceCheck(sb, curPos, temp);
                        curPos += charWidth[content[j]];
                        continue;
                    }

                    //�ų����������������
                    sb.Append(content[j]);
                    curPos += fontSize;
                }

                if (sb.Length != 0) PlaceCheck(sb, curPos, temp);
                sentenceInfos[i] = temp;
            }
            RefreshHighlight(0);
        }

        //ˢ�¸�ʾ�����ʾ
        public void RefreshDisplay()
        {
            //������е�ǰ��ʾ
            foreach (SentenceItem sentenceItem in sentences)
            {
                sentenceItem.ClearContent();
            }

            //����ǰ�����Ϣ���µ���ʾ��
            for (int i = 0; i < sentenceInfos.Count; i++)
            {
                sentences[i].SetContent(sentenceInfos[i]);
            }

            //���������Ƶ���һ���ʣ�����������С
            itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight * (sentenceInfos.Count));
            RefreshHighlight(0);
        }

        //�����¼�
        public void OnScroll(PointerEventData eventData)
        {
            if (eventData.scrollDelta.y > 0 && !playControlUI.isPlaying)
            {
                SaveCheckAndPhoneticPos();
                RefreshHighlight(concentrate - 1);
            }

            if (eventData.scrollDelta.y < 0 && !playControlUI.isPlaying)
            {
                SaveCheckAndPhoneticPos();
                RefreshHighlight(concentrate + 1);
            }
        }

        //ˢ�µ�ǰ�����ĸ��
        public void RefreshHighlight(int index)
        {
            if(index < 0) index = 0;
            if(index > sentenceInfos.Count - 1) index = sentenceInfos.Count - 1;
            if (sentenceInfos.Count == 0) return;

            concentrate = index;
            checkDisplayUI.InfoToCheck(sentenceInfos[index]);
            phoneticDisplayUI.InfoToPhonetic(sentenceInfos[index]);
            itemContainer.anchoredPosition = new Vector2(-960, itemHeight * index);
        }

        //�洢��ǰ���ӵ�ע���ͽ���λ����Ϣ���÷��������ʱ����Ϣ������
        public void SaveCheckAndPhoneticPos()
        {
            Sentence temp = sentenceInfos[concentrate];
            temp.checkPoints = checkDisplayUI.CheckToInfo();
            temp.phonetics = phoneticDisplayUI.PhoneticToInfo();
            sentenceInfos[concentrate] = temp;
        }

        public static void SetLog(string input)
        {
            ColorSelectUI.SetLogText(input);
        }
    }
}
