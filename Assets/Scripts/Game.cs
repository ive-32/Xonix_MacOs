using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject player;
    public GameObject field;
    
    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        Instantiate(player, transform);
        Instantiate(field, transform);
    }
}
