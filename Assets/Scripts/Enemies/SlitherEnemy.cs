using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private const float DefaultJerkDuration = 0.9f;
    private const float DefaultEnemySpeed = 0.3f;
    private const float DefaultJerkSpeed = 1.5f;

    private GameObject _body;
    private GameObject _podForward;
    private GameObject _podBackward;
    private float _jerkDuration;
    private bool _movingBody;
    private const float PodLength = EnemySize;
    private int _mainAngle;
    private Vector3 _forwardPodPosition;
    private Vector3 _backwardPodPosition;
    private List<Pod> _pods = new List<Pod>();
    
    
    protected override void Start()
    {
        base.Start();
        EnemySpeed = DefaultEnemySpeed;
        _body = transform.Find("Body")?.gameObject;

        _podForward = transform.Find("PseudoPod_Forward")?.gameObject;
        _podBackward = transform.Find("PseudoPod_Backward")?.gameObject;
        
        for (var i = 0; i < transform.childCount; i++)
        {
            var pod = transform.GetChild(i);
            if (pod.name == "PseudoPod")
            {
                var direction = transform.rotation * Vector3.left;
                _pods.Add(new Pod()
                {
                    PodObject = pod.gameObject,
                    Direction = direction, 
                    Speed = Random.Range(0.1f, 0.3f),
                    Posititon = direction * PodLength
                });
                
            }
        }

        //_forwardPodPosition = _bodyPosition + new Vector3(-PodLength, 0, 0);
        //_backwardPodPosition = _bodyPosition + new Vector3(PodLength, 0, 0);
    } 
    
    protected new void Update()
    {
        var position = Climb(Time.deltaTime);
        if (_jerkDuration <= 0 && IsStraightClimb(this.transform.position, 3))
        {
            /*if (EnemySpeed < DefaultJerkSpeed * 0.7f)
            {
                EnemySpeed += DefaultJerkSpeed * 0.3f;
                _jerkDuration = DefaultJerkDuration / 4;
            }
            else
            {
                EnemySpeed = DefaultEnemySpeed * DefaultJerkSpeed;
                _jerkDuration = DefaultJerkDuration;
                
            }*/
            EnemySpeed = DefaultJerkSpeed;
            _jerkDuration = DefaultJerkDuration;
        }
        
        if (_jerkDuration > 0)
            Jerk();

        /*_jerkDuration -= Time.deltaTime;
        EnemySpeed -= (DefaultEnemySpeed * DefaultJerkSpeed * 0.9f / DefaultJerkDuration) * Time.deltaTime;
        if (_jerkDuration <= 0)
        {
            EnemySpeed = DefaultEnemySpeed * DefaultJerkSpeed;
            _jerkDuration = DefaultJerkDuration;
        }*/
        transform.position = position;
    }

    private void Jerk()
    {
        _jerkDuration -= Time.deltaTime;
        EnemySpeed -= (DefaultJerkSpeed * 0.7f / DefaultJerkDuration) * Time.deltaTime;
        if (_jerkDuration <= 0 && !IsStraightClimb(this.transform.position, 3))
            EnemySpeed = DefaultEnemySpeed;
    }
    
    private void UpdateBody(Vector3 position, float deltaTime)
    {
       
        /*_podForward.transform.localPosition = _bodyPosition;
        _podBackward.transform.localPosition = _bodyPosition;
        _podForward.transform.localScale = new Vector3((_bodyPosition - _forwardPodPosition).magnitude / PodLength, 1, 1);
        _podBackward.transform.localScale = new Vector3((_bodyPosition - _backwardPodPosition).magnitude / PodLength, 1, 1);
        
        foreach (var pod in _pods)
        {
            pod.PodObject.transform.localPosition = _bodyPosition;
            var podLenght = pod.Posititon.magnitude; 
            if (podLenght > PodLength * 1.5f)
                pod.Direction = -pod.Direction;

            if (podLenght < PodLength * 0.5f)
            {
                pod.Direction = -pod.Direction;
                pod.Speed = Random.Range(0.3f, 1f);
            }

            pod.Posititon += pod.Direction * (pod.Speed * Time.deltaTime);

            pod.PodObject.transform.localScale = new Vector3(podLenght / PodLength, 1, 1);
        }*/

    }
    
    private static int GetAngleFromDirection(Vector3 direction)
    {
        if (direction == Vector3.left) return 270;
        if (direction == Vector3.up) return 180;
        if (direction == Vector3.right) return 90;
        if (direction == Vector3.down) return 0;
        
        return 0;
    }
    
    /// <summary>
    /// Check what can climb on Direction without any rotations
    /// </summary>
    /// <param name="position"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    private bool IsStraightClimb(Vector3 position, int distance)
    {
        for (var i = 0; i < distance; i++)
        {
            position += Direction;
            if (Field.GetTileType(position).IsGround() 
                || !Field.GetTileType(position + GetPositiveRotation(Direction)).IsGround())
                return false;
        }

        return true;
    }

}