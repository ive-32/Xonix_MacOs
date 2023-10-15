using TMPro;
using UnityEngine;

public class UiSplashLabel : UiLabel
{
    private Animation _animation;

    public override void Awake()
    {
        _animation = GetComponent<Animation>();

        _animation.enabled = false;
    }
}
