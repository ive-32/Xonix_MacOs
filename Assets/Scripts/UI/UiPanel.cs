using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class UiPanel : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI MainText;
    
    public void OnClick()
    { 
        Destroy(this.GameObject());
    }

    public void SetMainText(string text)
    {
        MainText.SetText(text);
    }

    public void SetButtonText(string text)
    {
        ButtonText.SetText(text);
    }
}
