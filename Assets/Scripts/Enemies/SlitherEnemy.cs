using System.Collections.Generic;
using UnityEngine;

public class SlitherEnemy : ClimberEnemy
{
    private class Pod
    {
        public GameObject PodObject;
        public Vector3 Direction;
        public float Speed;
        public Vector3 Posititon;
    }
    
    
    private Transform slitherBody;

    private GameObject _body;
    private GameObject _podForward;
    private GameObject _podBackward;
    private Vector3 _bodyPosition;
    private Vector3 _previousPosition;
    private bool _movingBody;
    private const float _podLegth = 0.5f;
    private int _mainAngle;
    private Vector3 _forwardPodPosition;
    private Vector3 _backwardPodPosition;
    private List<Pod> _pods = new List<Pod>();
    
    
    protected override void Start()
    {
        base.Start();
        EnemySpeed = 0.5f;
        _body = transform.Find("Body")?.gameObject;

        _podForward = transform.Find("PseudoPod_Forward")?.gameObject;
        _podBackward = transform.Find("PseudoPod_Backward")?.gameObject;
        
        for (var i = 0; i < transform.childCount; i++)
        {
            var pod = transform.GetChild(i);
            if (pod.name == "PseudoPod")
            {
                //var direction = Random.insideUnitCircle.normalized;
                //var direction = Quaternion.AngleAxis(180.0f / (transform.childCount - 3) + 225, Vector3.forward) * Vector3.left;
                var direction = transform.rotation * Vector3.left;
                _pods.Add(new Pod()
                {
                    PodObject = pod.gameObject,
                    Direction = direction, 
                    Speed = Random.Range(0.1f, 0.3f),
                    Posititon = direction * _podLegth
                });
                
            }
        }

        _bodyPosition = Vector3.zero;
        _forwardPodPosition = _bodyPosition + new Vector3(-_podLegth, 0, 0);
        _backwardPodPosition = _bodyPosition + new Vector3(_podLegth, 0, 0);
        _previousPosition = transform.position;
    } 
    
    protected override void Update()
    {
        base.Update();
        var glueTo = GetPositiveRotation(_direction);
        _mainAngle = GetAngleFromDirection(glueTo);
        transform.rotation = Quaternion.AngleAxis(_mainAngle, Vector3.forward);
        UpdateBody();
    }

    private void UpdateBody()
    {
        var globalPos = transform.position;
        var step = (globalPos - _previousPosition).magnitude;

        if (step > _podLegth)
        {
            _movingBody = !_movingBody;
            _previousPosition = globalPos;
        }

        if (_movingBody)
        {
            var bodyStep = new Vector3(-1, 0, 0) * (Time.deltaTime * EnemySpeed * 5);
            _bodyPosition += bodyStep;
            _forwardPodPosition -= bodyStep;
        }
        else
        {
            _forwardPodPosition = new Vector3(-_podLegth, 0, 0);
            _bodyPosition = new Vector3(step, -0.3f, 0);
            _backwardPodPosition = _bodyPosition + new Vector3(_podLegth, 0, 0);
        }
        
        _body.transform.localPosition = _bodyPosition;
        _podForward.transform.localPosition = _bodyPosition;
        _podBackward.transform.localPosition = _bodyPosition;
        _podForward.transform.localScale = new Vector3((_bodyPosition - _forwardPodPosition).magnitude / _podLegth, 1, 1);
        _podBackward.transform.localScale = new Vector3((_bodyPosition - _backwardPodPosition).magnitude / _podLegth, 1, 1);
        
        foreach (var pod in _pods)
        {
            pod.PodObject.transform.localPosition = _bodyPosition;
            var podLenght = pod.Posititon.magnitude; 
            if (podLenght > _podLegth * 1.5f)
                pod.Direction = -pod.Direction;

            if (podLenght < _podLegth * 0.5f)
            {
                pod.Direction = -pod.Direction;
                pod.Speed = Random.Range(0.3f, 1f);
            }

            pod.Posititon += pod.Direction * (pod.Speed * Time.deltaTime);

            pod.PodObject.transform.localScale = new Vector3(podLenght / _podLegth, 1, 1);
        }
        
    }
    
    private static int GetAngleFromDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.left) return 270;
        if (direction == Vector2Int.up) return 180;
        if (direction == Vector2Int.right) return 90;
        if (direction == Vector2Int.down) return 0;
        
        return 0;
    }
}