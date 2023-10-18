using System;
using UnityEngine;

public class ProgressCircle : MonoBehaviour
{
    private float _timeToLive = 20;
    private float _leftTime = 5;
    private const int NumPoints = 10;
    private GameObject _point;
    private int _currentPercent = NumPoints;
    private readonly SpriteRenderer[] _sprites = new SpriteRenderer[NumPoints + 1];
    private readonly GameObject[] _points = new GameObject[NumPoints + 1];
    
    private void Awake()
    {
        _point = transform.Find("Point").gameObject;
        for (var i = 1; i < NumPoints; i++)
            Instantiate(_point, transform);

        const float stepAngle = (float) (360 - 90) / (NumPoints - 1);
        for (var i = 0; i < transform.childCount; i++)
        {
            _points[i + 1] = transform.GetChild(i).gameObject;
            _points[i + 1].transform.localPosition = Quaternion.AngleAxis(225 - i * stepAngle, Vector3.forward) * (Vector3.right * 0.6f);
            _sprites[i + 1] = _points[i + 1].GetComponent<SpriteRenderer>();
        }
    }

    public void SetTimeToLive(float time)
    {
        _timeToLive = time;
        _leftTime = time;
    }
    
    private void Start()
    {
        _leftTime = _timeToLive;
        _currentPercent = NumPoints;
    }

    private void Update()
    {
        _leftTime -= Time.deltaTime;

        var realPercent = _leftTime / _timeToLive;
        var percent = Mathf.CeilToInt(realPercent * NumPoints);

        _currentPercent = percent;
        
        var color = realPercent > 0.5f 
            ? new Color(1 - (realPercent - 0.5f) * 2, 1, 0.3f)
            : new Color(1, realPercent * 2, 0.3f);
        
        for (var i = 1; i <= NumPoints; i++)
        {
            _sprites[i].color = i<= percent ? color : new Color(0.3f, 0.3f, 0.3f);
        }
    }
}