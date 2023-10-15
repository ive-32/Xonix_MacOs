using TMPro;
using UnityEngine;

public class UiLabel : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private string _currentText;
    
    public virtual void Awake()
    {
        _text = transform.GetComponentInChildren<TextMeshProUGUI>();
        _currentText = string.Empty;
    }

    public void SetText(string text)
    {
        if (_currentText == text) return;
        
        _text.SetText(text);
        _currentText = text;
    }
}
