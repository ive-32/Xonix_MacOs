using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("player")] public GameObject playerPrefab;
    [FormerlySerializedAs("field")] public GameObject fieldPrefab;
    [FormerlySerializedAs("enemies")] public GameObject enemiesPrefab;

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        var fieldObject = Instantiate(fieldPrefab, transform);
        var playerObject = Instantiate(playerPrefab, transform);
        var enemiesObject = Instantiate(enemiesPrefab, transform);

        playerObject.GetComponent<Player>()._field = fieldObject.GetComponent<Field>();
        fieldObject.GetComponent<Field>().Enemies = enemiesObject.GetComponent<Enemies>();
        enemiesObject.GetComponent<Enemies>().Field = fieldObject.GetComponent<Field>();
    }
}
