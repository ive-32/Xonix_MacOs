using UnityEngine;

public class UiSplashLabel : UiLabel
{
    private float _timeToLive = 1.0f;

    private void Update()
    {
        _timeToLive -= Time.deltaTime;
        if (_timeToLive <= 0)
            Destroy(this.gameObject);
    }
}
