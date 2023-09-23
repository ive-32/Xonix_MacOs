using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("player")] public GameObject playerPrefab;
    [FormerlySerializedAs("field")] public GameObject fieldPrefab;
    //[FormerlySerializedAs("enemies")] public List<GameObject> enemiesPrefabs;

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        var fieldObject = Instantiate(fieldPrefab, transform);
        var playerObject = Instantiate(playerPrefab, transform);

        playerObject.GetComponent<Player>()._field = fieldObject.GetComponent<Field>();
    }
}
