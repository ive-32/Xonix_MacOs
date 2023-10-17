using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("player")] public GameObject playerPrefab;
    [FormerlySerializedAs("field")] public GameObject fieldPrefab;
    [FormerlySerializedAs("enemies")] public GameObject enemiesPrefab;
    [FormerlySerializedAs("uiLabel")]public GameObject uILabelPrefab;
    [FormerlySerializedAs("uiPanel")]public GameObject uIPanelPrefab;
    [FormerlySerializedAs("uiSplash")]public GameObject uISplashTextPrefab;
    [FormerlySerializedAs("bonuses")]public GameObject bonusesPrefab;

    private Field _field;
    private Enemies _enemies;
    private Bonuses _bonuses;
    private GameObject _scoresLabel;
    private GameObject _livesLabel;
    private GameObject _filledLabel;
    private GameObject _playerObject;
    private GameObject _labelObject;

    private UiLabel _scoresText;
    private UiLabel _livesText;
    private UiLabel _filledText;

    private UiPanel _panel;

    private GameState _gameState = GameState.SplashScreen;
    
    void Start()
    {
        ShowSplashScreen();
    }

    private void Update()
    {
        if (_gameState is GameState.GameOver or GameState.LevelCompleted)
        {
            IcwGame.GameSpeed -=  Time.deltaTime * 5 / 2.0f;
            if (IcwGame.GameSpeed > 0) return;
            
            if (_gameState == GameState.GameOver)
                ShowSplashScreen();
            if (_gameState == GameState.LevelCompleted)
                NextLevel();
            
            return;
        }

        if (_gameState is GameState.NextLevelScreen)
        {
            IcwGame.GameSpeed +=  Time.deltaTime * 5;
            if (IcwGame.GameSpeed < IcwGame.DefaultGameSpeed) return;
    
            LevelStart();
        }

        _scoresText.SetText($"Scores: {IcwGame.Scores}");
        _livesText.SetText($"Lives: {IcwGame.Lives}");
        _filledText.SetText($"Filled: {IcwGame.Filled}% / 80%");    
        
        if (IcwGame.Lives == 0)
            GameOver();

        IcwGame.Filled = _field.GetFillPercents();

        if (IcwGame.Filled >= 80)
        {
            LevelCompleted();
        }
    }

    private void ShowPanel(string text = null)
    {
        var panel = Instantiate(uIPanelPrefab, new Vector3(IcwGame.SizeX / 2.0f, IcwGame.SizeY / 4.0f, -1.0f),
            Quaternion.identity, transform);
        _panel = panel.GetComponent<UiPanel>();
        
        _panel.OnClickMethod += NextLevel;
        
        if (text is null)
            _panel.SetMainText("Move across the field to capture territory. Capture 80%." + 
                               Environment.NewLine + 
                               "Avoid enemies from crossing your trail. Have Fun!" 
                               );
        else 
            _panel.SetMainText(text);
    }

    private void BuildGame()
    {
        foreach (Transform child in transform)
            Destroy(child.GameObject());

        var fieldObject = Instantiate(fieldPrefab, transform);
        var enemiesObject = Instantiate(enemiesPrefab, transform);
        var bonusesObject = Instantiate(bonusesPrefab, transform);

        _field = fieldObject.GetComponent<Field>();
        _bonuses = bonusesObject.GetComponent<Bonuses>();
        _enemies = enemiesObject.GetComponent<Enemies>();
        _field.Enemies = _enemies;
        _field.SplashTextPrefab = uISplashTextPrefab;
        
        _enemies.Field = _field;
        _bonuses.SplashTextPrefab = uISplashTextPrefab;
        
        _scoresLabel = Instantiate(uILabelPrefab, new Vector3(3, IcwGame.SizeY, 0), Quaternion.identity, transform);
        _livesLabel = Instantiate(uILabelPrefab, new Vector3(13, IcwGame.SizeY, 0), Quaternion.identity, transform);
        _filledLabel = Instantiate(uILabelPrefab, new Vector3(23, IcwGame.SizeY, 0), Quaternion.identity, transform);

        _scoresText = _scoresLabel.GetComponent<UiLabel>();
        _livesText = _livesLabel.GetComponent<UiLabel>();
        _filledText = _filledLabel.GetComponent<UiLabel>();
        
        IcwGame.Scores = 0;
        IcwGame.Lives = 5;
        IcwGame.Filled = 0;

    }

    private void AddPlayer()
    {
        _playerObject = Instantiate(playerPrefab, transform);
        var player = _playerObject.GetComponent<Player>();
        player.Field = _field;
        player.Bonuses = _bonuses;
        for (var i = 0; i < _enemies.transform.childCount; i++)
        {
            var enemy = _enemies.transform.GetChild(i).GetComponent<BaseEnemy>();
            if (enemy is ClimberEnemy)
                player.ContactEnemies.Add(enemy as ClimberEnemy);
        }
        _field.Enemies.SetPlayer(_playerObject);
    }
    
    private void ShowLabel(string text)
    {
        _labelObject = Instantiate(uISplashTextPrefab, new Vector3(IcwGame.SizeX / 2.0f, IcwGame.SizeY / 2.0f , 0), 
            Quaternion.identity, transform);
        var textObject = _labelObject.GetComponent<UiLabel>();
        textObject.SetText(text, true);
    }

    private void ShowSplashScreen()
    {
        IcwGame.Level = 0;
        IcwGame.GameSpeed = IcwGame.DefaultGameSpeed;
        _gameState = GameState.SplashScreen;
        BuildGame();
        ShowPanel();
    }
    
    private void LevelStart()
    {
        _gameState = GameState.InGame;
        IcwGame.GameSpeed = IcwGame.DefaultGameSpeed;
        AddPlayer();
    }
    
    private void NextLevel()
    {
        _gameState = GameState.NextLevelScreen;
        IcwGame.GameSpeed = 0;
        IcwGame.Level++;
        BuildGame();
        ShowLabel($"Level {IcwGame.Level}");
    }

    private void GameOver()
    {
        Destroy(_playerObject);
        _gameState = GameState.GameOver;
        ShowLabel("Game Over");
    }

    private void LevelCompleted()
    {
        Destroy(_playerObject);
        _gameState = GameState.LevelCompleted;
        ShowLabel("Level completed");
    }

}
