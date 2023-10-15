using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseBonus : MonoBehaviour, IBonus
{
    private float _timeToLive = 5.0f;
    private BonusState _bonusState;
    private Vector3 _displayPosition = new (IcwGame.SizeX - 1, IcwGame.SizeY, 0);

    protected Player Player { get; set; }

    public GameObject SplashTextPrefab { get; set; }
    public BonusType BonusType { get; set; } = BonusType.Empty;

    public virtual void Start()
    {
        _displayPosition = new Vector3(IcwGame.SizeX - 1 - (int)BonusType, IcwGame.SizeY, 0);
        _bonusState = BonusState.BonusOnField;
        transform.localPosition = new Vector3(Random.Range(3, IcwGame.SizeX - 3), Random.Range(3, IcwGame.SizeY - 3)).GetCenterTile();
    }

    public void Update()
    {
        if (_timeToLive <= 0)
        {
            switch(_bonusState)
            {
                case BonusState.BonusOnField : Destroy(this.gameObject); break;
                case BonusState.BonusPickedUp : OnBonusEndMethod(); break;
                default: Destroy(this.gameObject); break;
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
        
        _timeToLive = 5.0f;   
        _bonusState = BonusState.BonusPickedUp;
        transform.localPosition = _displayPosition;
    }

    public virtual void OnBonusEndMethod()
    {
        Destroy(this.gameObject);
    }
    
}