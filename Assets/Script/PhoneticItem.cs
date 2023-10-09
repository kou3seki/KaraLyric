using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneticItem : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public PhoneticDisplayUI phoneticDisplayUI;
    public Text text;
    public int pos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int pos)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        SetPos(pos);
    }

    public void Init(int pos, string content)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        SetPos(pos);
        text.text = content;
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        text.text = "";
        SetPos(0);
        transform.SetAsFirstSibling();
    }

    public void SetPos(int pos)
    {
        this.pos = pos;
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Clear();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            phoneticDisplayUI.inputField.ActivateInputField();
            phoneticDisplayUI.current = this;
        }
    }
}
