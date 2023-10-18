using Unity.Mathematics;
using UnityEngine;

public class BaseBonus : MonoBehaviour, IBonus
{
    private float _timeToLive = 5.0f;
    private BonusState _bonusState;
    private Vector3 _displayPosition = new (IcwGame.SizeX - 1, IcwGame.SizeY, 0);
    private ProgressCircle _progress;

    protected Player Player { get; set; }

    public GameObject SplashTextPrefab { get; set; }
    public BonusType BonusType { get; set; } = BonusType.Empty;
    public GameObject ProgressPrefab { get; set; }
    
    public virtual void Start()
    {
        _displayPosition = new Vector3(IcwGame.SizeX - 1 - (int)BonusType, IcwGame.SizeY, 0);
        _bonusState = BonusState.BonusOnField;
        var progress = Instantiate(ProgressPrefab, transform);
        _progress = progress.GetComponent<ProgressCircle>();

        if (_progress != null)
            _progress.SetTimeToLive(_timeToLive);
    }

    public void Update()
    {
        if (_timeToLive <= 0)
        {
            switch(_bonusState)
            {
                case BonusState.BonusOnField : Destroy(gameObject); break;
                case BonusState.BonusPickedUp : OnBonusEndMethod(); break;
                default: Destroy(gameObject); break;
            }
        }

        _timeToLive -= Time.deltaTime;
    }
    
    public virtual void OnPickedUp(Player player)
    {
        if (SplashTextPrefab is not null)
        {
            var splashText = Instantiate(SplashTextPrefab, transform.localPosition, quaternion.identity);
            splashText.GetComponent<UiSplashLabel>().SetText(BonusType.GetLabel());
        }
        
        _timeToLive = 10.0f;   
        _bonusState = BonusState.BonusPickedUp;
        transform.localPosition = _displayPosition;
        
        if (_progress != null)
            _progress.SetTimeToLive(_timeToLive);
    }

    public virtual void OnBonusEndMethod()
    {
        Destroy(this.gameObject);
    }
    
}