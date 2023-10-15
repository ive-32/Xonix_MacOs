using System;
using TMPro;
using UnityEngine;

public class UiPanel : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI MainText;
    public delegate void OnClickDelegate();

    [NonSerialized] public OnClickDelegate OnClickMethod;
    
    public void OnClick()
    { 
        OnClickMethod?.Invoke();
        Destroy(this.gameObject);
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
