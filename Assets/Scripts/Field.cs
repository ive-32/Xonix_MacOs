using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Field : MonoBehaviour
{
    [FormerlySerializedAs("TilesList")] public List<GameObject> tilesList = new List<GameObject>();
    
    private GameObject _borderTile; 
    
    void Start()
    {
        _borderTile = tilesList[0];

        for (var i = 0; i < IcwGame.SizeX; i++)
        {
            Instantiate(_borderTile, new Vector3(i, 0, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(i, 1, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(i, IcwGame.SizeY - 2, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(i, IcwGame.SizeY - 1, 0), Quaternion.identity, this.transform);
        }

        for (var j = 2; j < IcwGame.SizeY - 2; j++)
        {
            Instantiate(_borderTile, new Vector3(0, j, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(1, j, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(IcwGame.SizeX - 2, j, 0), Quaternion.identity, this.transform);
            Instantiate(_borderTile, new Vector3(IcwGame.SizeX - 1, j, 0), Quaternion.identity, this.transform);
        }
    }

    
}
