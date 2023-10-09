using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneticDisplayUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject phoneticGo;
    [SerializeField] RectTransform selfRect;
    [SerializeField] Button switchMode;
    public InputField inputField;

    PhoneticItem[] phoneticItems;
    [HideInInspector] public PhoneticItem current;

    // Start is called before the first frame update
    void Start()
    {
        phoneticItems = new PhoneticItem[20];
        for (int i = 0; i < 20; i++)
        {
            GameObject temp = Instantiate(phoneticGo, selfRect);
            phoneticItems[i] = temp.GetComponent<PhoneticItem>();
            phoneticItems[i].phoneticDisplayUI = this;
            temp.SetActive(false);
        }

        switchMode.onClick.AddListener(SwitchMode);
        inputField.onValueChanged.AddListener(OnInput);
        inputField.onEndEdit.AddListener(OnEndInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter) && current != null)
        {
            current = null;
            inputField.DeactivateInputField();
        }
    }

    public List<Phonetic> PhoneticToInfo()
    {
        List<Phonetic> res = new List<Phonetic>();

        for (int i = selfRect.childCount - 1; i >=0; i--)
        {
            GameObject tempGo = selfRect.GetChild(i).gameObject;
            if (!tempGo.activeInHierarchy) break;
            PhoneticItem temp = tempGo.GetComponent<PhoneticItem>();
            res.Add(new Phonetic(temp.text.text, temp.pos));
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

    public void SwitchMode()
    {
        transform.SetAsFirstSibling();
    }

    public void OnInput(string input)
    {
        if(current != null)
        {
            current.text.text = input;
        }
    }

    public void OnEndInput(string input)
    {
        if (input.Equals("") && current != null)
        {
            current.Clear();
            current = null;
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PhoneticItem temp = selfRect.GetChild(0).gameObject.GetComponent<PhoneticItem>();
            temp.Init((int)Input.mousePosition.x - 960);
            current = temp;
            inputField.ActivateInputField();
        }
    }
}
