using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RASettingButton : MonoBehaviour
{
    public eSettingCategory buttonType;
    public TMP_Text buttonText;
    public RAButton button;

    public void Init(eSettingCategory _type)
    {
        buttonType = _type;
        buttonText.SetText(StringManager.GetLocalizeString(_type.ToString()));

	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
