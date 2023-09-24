using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("player")] public GameObject playerPrefab;
    [FormerlySerializedAs("field")] public GameObject fieldPrefab;
    [FormerlySerializedAs("enemies")] public GameObject enemiesPrefab;
    [FormerlySerializedAs("uilabel")]public GameObject uILabelPrefab;
    [FormerlySerializedAs("uipanel")]public GameObject uIPanelPrefab;

    private Field _field;
    private GameObject _scoresLabel;
    private GameObject _livesLabel;
    private GameObject _filledLabel;

    private UiLabel _scoresText;
    private UiLabel _livesText;
    private UiLabel _filledText;

    private UiPanel _panel;

    private float lastCalculatedPercent;
    
    void Start()
    {
        BuildGame(true);
        ShowPanel();
    }

    private void Update()
    {
        _scoresText.SetText($"Scores: {IcwGame.Scores}");
        _livesText.SetText($"Lives: {IcwGame.Lives}");
        _filledText.SetText($"Filled: {IcwGame.Filled}% / 80%");    
        if (IcwGame.Lives == 0)
            GameOver();

        if (Time.time - lastCalculatedPercent > 0.1f)
        {
            lastCalculatedPercent = Time.time;
            IcwGame.Filled = _field.GetFillPercents();
        }
    }

    private void ShowPanel()
    {
        var panel = Instantiate(uIPanelPrefab, new Vector3(IcwGame.SizeX / 2.0f, IcwGame.SizeY / 2.0f, -1.0f),
            Quaternion.identity, transform);
        _panel = panel.GetComponent<UiPanel>();
        _panel.OnClickMethod += StartGame;
        _panel.SetMainText("cut the field" + Environment.NewLine + "avoid enemies");
    }

    private void BuildGame(bool isSplashScreen = false)
    {
        foreach (Transform child in transform)
            Destroy(child.GameObject());

        var fieldObject = Instantiate(fieldPrefab, transform);
        var enemiesObject = Instantiate(enemiesPrefab, transform);

        _field = fieldObject.GetComponent<Field>(); 

        _field.Enemies = enemiesObject.GetComponent<Enemies>();
        enemiesObject.GetComponent<Enemies>().Field = _field;
        
        if (!isSplashScreen)
        {
            var playerObject = Instantiate(playerPrefab, transform);
            playerObject.GetComponent<Player>()._field = _field;
        }
        
        _scoresLabel = Instantiate(uILabelPrefab, new Vector3(3, IcwGame.SizeY, 0), Quaternion.identity, transform);
        _livesLabel = Instantiate(uILabelPrefab, new Vector3(13, IcwGame.SizeY, 0), Quaternion.identity, transform);
        _filledLabel = Instantiate(uILabelPrefab, new Vector3(23, IcwGame.SizeY, 0), Quaternion.identity, transform);

        _scoresText = _scoresLabel.GetComponent<UiLabel>();
        _livesText = _livesLabel.GetComponent<UiLabel>();
        _filledText = _filledLabel.GetComponent<UiLabel>();
        
        IcwGame.Scores = 0;
        IcwGame.Lives = 5;
        IcwGame.Filled = 0;

        lastCalculatedPercent = 0f;
    }
    
    private void StartGame()
    {
        BuildGame();
    }
    
    private void GameOver()
    {
        BuildGame(true);
        ShowPanel();
    }
}
