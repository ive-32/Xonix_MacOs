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

    private readonly FieldTile[] _field = new FieldTile[IcwGame.SizeX * IcwGame.SizeY];

    public void Awake()
    {
        for (var i = 0; i < IcwGame.SizeX * IcwGame.SizeY ; i++)
            _field[i] = new FieldTile();
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

    public TileType GetTile(int x, int y)
        => _field[x * IcwGame.SizeY + y].TileType;

    public TileType GetTile(Vector2Int pos)
        => _field[pos.x * IcwGame.SizeY + pos.y].TileType;

    public void PutTile(TileType tileType, Vector2Int pos)
        => PutTile(tileType, pos.x, pos.y);

    public void PutTile(TileType tileType, int x, int y)
    {
        var tile = GetTileByType(tileType);
        
        if (_field[x * IcwGame.SizeY + y].GameObject is not null)
            Destroy(_field[x * IcwGame.SizeY + y].GameObject);
        
        if (tile is not null)
            _field[x * IcwGame.SizeY + y].GameObject = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, transform);

        _field[x * IcwGame.SizeY + y].TileType = tileType;
    }

    private GameObject GetTileByType(TileType tileType)
        => tileType switch
        {
            TileType.Border => tilesList[0],
            TileType.Filled => tilesList[1],
            TileType.Trace => tilesList[2],
            _ => null
        };

    #region FillField

    private static int _fillRoutineDeepLevel;
    private readonly int[,] _tmpFieldProjection = new int[IcwGame.SizeX, IcwGame.SizeY];
    private readonly List<Vector3Int> _startFillPoints = new ();

    private void FillFromPoint(Vector3Int startPoint, int value)
    {
        _fillRoutineDeepLevel++;
        
        if (startPoint.x is >= IcwGame.SizeX or < 0 || startPoint.y is >= IcwGame.SizeY or < 0) 
            return;
        
        if (_tmpFieldProjection[startPoint.x, startPoint.y] != 0) 
            return;
        
        _tmpFieldProjection[startPoint.x, startPoint.y] = value;
        
        var tmpVector = Vector3.up;
        
        for (var i = 0; i < 9; i++)
        {
            tmpVector = Quaternion.AngleAxis(45, Vector3.forward) * tmpVector;
            var tmpVectorInt = Vector3Int.RoundToInt(tmpVector);
            
            if (_tmpFieldProjection[startPoint.x + tmpVectorInt.x, startPoint.y + tmpVectorInt.y] != 0) 
                continue;
            
            if (_fillRoutineDeepLevel < 100) FillFromPoint(tmpVectorInt + startPoint, value);
            else _startFillPoints.Add(tmpVectorInt + startPoint);
        }
        
        _fillRoutineDeepLevel--;
    }


    #endregion
    
}
