using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("player")] public GameObject playerPrefab;
    [FormerlySerializedAs("field")] public GameObject fieldPrefab;
    [FormerlySerializedAs("enemies")] public GameObject enemiesPrefab;
    [FormerlySerializedAs("uilabel")]public GameObject uILabelPrefab;

    private GameObject _scoresLabel;
    private GameObject _livesLabel;

    private UiLabel _scoresText;
    private UiLabel _livesText;
    
    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        _scoresText.SetText($"Scores: {IcwGame.Scores}");
        _livesText.SetText($"Lives: {IcwGame.Lives}");
    }

    private void StartGame()
    {
        var fieldObject = Instantiate(fieldPrefab, transform);
        var playerObject = Instantiate(playerPrefab, transform);
        var enemiesObject = Instantiate(enemiesPrefab, transform);

        playerObject.GetComponent<Player>()._field = fieldObject.GetComponent<Field>();
        fieldObject.GetComponent<Field>().Enemies = enemiesObject.GetComponent<Enemies>();
        enemiesObject.GetComponent<Enemies>().Field = fieldObject.GetComponent<Field>();

        _scoresLabel = Instantiate(uILabelPrefab, new Vector3(3, IcwGame.SizeY, 0), Quaternion.identity, transform);
        _livesLabel = Instantiate(uILabelPrefab, new Vector3(13, IcwGame.SizeY, 0), Quaternion.identity, transform);

        _scoresText = _scoresLabel.GetComponent<UiLabel>();
        _livesText = _livesLabel.GetComponent<UiLabel>();
        
        IcwGame.Scores = 0;
        IcwGame.Lives = 5;
    }
}
