using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneticText : MonoBehaviour
{
    public Text text;
    [HideInInspector] public LyricPlayUI lyricPlayUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(string content, float pos, Color color, Transform parent)
    {
        gameObject.SetActive(true);
        text.color = color;
        text.text = content;
        transform.SetParent(parent);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, 70);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        transform.SetParent(lyricPlayUI.poolContent);
    }
}
