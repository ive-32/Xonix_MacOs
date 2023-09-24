using TMPro;
using UnityEngine;

public class UiLabel : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called before the first frame update

    public void SetText(string text)
        => _text.SetText(text);
}
