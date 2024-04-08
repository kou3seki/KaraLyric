using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ver2
{
    public class PhoneticItem : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public PhoneticDisplayUI phoneticDisplayUI;
        public Text text;
        public int pos;
        Image image;

        // Start is called before the first frame update
        void Awake()
        {
            image = GetComponent<Image>();
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

        public void SetIsActive(bool active)
        {
            if (!active) image.color = new Color(0.4f, 0.4f, 0.4f);
            else image.color = new Color(0.9f, 0.9f, 0.9f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Clear();
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                phoneticDisplayUI.SetCurrentInput(this);
            }
        }
    }

}
