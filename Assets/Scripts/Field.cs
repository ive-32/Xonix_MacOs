using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Field : MonoBehaviour
{
    [FormerlySerializedAs("TilesList")] public List<GameObject> tilesList = new List<GameObject>();

    private class FieldTile
    {
        public TileType TileType = TileType.Empty;
        public GameObject GameObject;
    };

    private readonly FieldTile[,] _field = new FieldTile[IcwGame.SizeX, IcwGame.SizeY];

    public void Awake()
    {
        for (var i = 0; i < IcwGame.SizeX; i++)
            for (var j = 0; j < IcwGame.SizeY; j++)
                _field[i, j] = new FieldTile();
    }

    public void Start()
    {
        BuildField();
    }

    public void BuildField()
    {
        for (var i = 0; i < IcwGame.SizeX; i++)
        {
            PutTile(TileType.Border, i, 0);
            PutTile(TileType.Border, i, 1);
            PutTile(TileType.Border, i, IcwGame.SizeY - 2);
            PutTile(TileType.Border, i, IcwGame.SizeY - 1);
        }

        for (var j = 2; j < IcwGame.SizeY - 2; j++)
        {
            PutTile(TileType.Border, 0, j);
            PutTile(TileType.Border, 1, j);
            PutTile(TileType.Border, IcwGame.SizeX - 2, j);
            PutTile(TileType.Border, IcwGame.SizeX - 1, j);
        }
    }
    
    public void PutTile(TileType tileType, int x, int y)
    {
        var tile = GetTileByType(tileType);
        
        if (_field[x, y].GameObject != null)
            Destroy(_field[x, y].GameObject);
        
        if (tile != null)
            _field[x, y].GameObject = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, transform);

        _field[x, y].TileType = tileType;
    }

    private GameObject GetTileByType(TileType tileType)
        => tileType switch
        {
            TileType.Border => tilesList[0],
            TileType.Filled => tilesList[1],
            TileType.Trace => tilesList[2],
            _ => null
        };
}
