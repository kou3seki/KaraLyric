using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectUI : MonoBehaviour
{
    [SerializeField] GameObject checkPanelGo;
    [HideInInspector] public CheckPanelUI checkPanelUI;

    public Button[] colors;

    // Start is called before the first frame update
    void Start()
    {
        checkPanelUI = checkPanelGo.GetComponent<CheckPanelUI>();
        colors[0].onClick.AddListener(delegate { SetColor(0); });
        colors[1].onClick.AddListener(delegate { SetColor(1); });
        colors[2].onClick.AddListener(delegate { SetColor(2); });
        colors[3].onClick.AddListener(delegate { SetColor(3); });
        colors[4].onClick.AddListener(delegate { SetColor(4); });
        colors[5].onClick.AddListener(delegate { SetColor(5); });
        colors[6].onClick.AddListener(delegate { SetColor(6); });
        colors[7].onClick.AddListener(delegate { SetColor(7); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(int index)
    {
        Color tempColor = colors[index].GetComponent<Image>().color;
        Sentence temp = checkPanelUI.sentenceInfos[checkPanelUI.concentrate];
        temp.color = tempColor;
        checkPanelUI.sentenceInfos[checkPanelUI.concentrate] = temp;
        checkPanelUI.sentences[checkPanelUI.concentrate].contentText.color = tempColor;
    }
}
