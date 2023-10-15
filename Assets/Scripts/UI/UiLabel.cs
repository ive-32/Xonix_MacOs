using TMPro;
using UnityEngine;

public class UiLabel : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private string _currentText;
    private GameObject _background;
    
    public void Awake()
    {
        _text = transform.GetComponentInChildren<TextMeshProUGUI>();
        _background = transform.Find("Background")?.gameObject;
        _currentText = string.Empty;
    }

    public void SetText(string text, bool onBackGround = false)
    {
        if (_currentText == text) return;
        _background.SetActive(onBackGround);
        
        _text.SetText(text);
        _currentText = text;
    }
}
