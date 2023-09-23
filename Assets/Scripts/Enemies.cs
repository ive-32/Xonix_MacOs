using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public List<GameObject> enemiesPrefabs = new ();
    [NonSerialized] public Field Field;
    
    // Start is called before the first frame update
    void Start()
    {
        var enemy = Instantiate(enemiesPrefabs[0], transform);
        enemy.GetComponent<BaseEnemy>().Field = Field;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
