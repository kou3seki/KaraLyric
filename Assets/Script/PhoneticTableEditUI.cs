using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ver2;

public class PhoneticTableEditUI : MonoBehaviour
{
    [SerializeField] InputField input;
    [SerializeField] Button addBtn;
    [SerializeField] Button saveBtn;
    // Start is called before the first frame update
    void Start()
    {
        addBtn.onClick.AddListener(delegate { PhoneticTable.Add(input.text); });
        saveBtn.onClick.AddListener(delegate { PhoneticTable.Save(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
