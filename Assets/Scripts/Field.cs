using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Field : MonoBehaviour
{
    [FormerlySerializedAs("TilesList")] public List<GameObject> tilesList = new List<GameObject>();
    [NonSerialized] public Enemies Enemies;
    
    private readonly FieldTile[] _field = new FieldTile[IcwGame.SizeX * IcwGame.SizeY];

    public void Awake()
    {
        for (var i = 0; i < IcwGame.SizeX * IcwGame.SizeY ; i++)
            _field[i] = new FieldTile(i / IcwGame.SizeY, i % IcwGame.SizeY);
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

    public TileType GetTileType(int x, int y)
        => _field[x * IcwGame.SizeY + y].TileType;

    public TileType GetTileType(Vector2Int pos)
        => _field[pos.x * IcwGame.SizeY + pos.y].TileType;

    public FieldTile GetTile(Vector2Int pos)
        => _field[pos.x * IcwGame.SizeY + pos.y];
    
    public void PutTile(TileType tileType, Vector2Int pos)
        => PutTile(tileType, pos.x, pos.y);

    public void PutTile(TileType tileType, int x, int y, float destroyTime = 0.0f)
    {
        var tile = GetTileByType(tileType);
        
        if (_field[x * IcwGame.SizeY + y].GameObject is not null )
        {
            if (destroyTime > 0)
                Destroy(_field[x * IcwGame.SizeY + y].GameObject, destroyTime);
            else 
                Destroy(_field[x * IcwGame.SizeY + y].GameObject);
        }        
        if (tile is not null)
            _field[x * IcwGame.SizeY + y].GameObject = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, transform);

        _field[x * IcwGame.SizeY + y].TileType = tileType;
    }

    public static bool IsPositionValid(float x, float y)
        => IsPositionValid(Mathf.RoundToInt(x), Mathf.RoundToInt(y)); 
    
    public static bool IsPositionValid(int x, int y)
        => x is >= 0 and < IcwGame.SizeX && y is >= 0 and < IcwGame.SizeY;
    public static bool IsPositionValid(Vector2Int pos)
        => IsPositionValid(pos.x, pos.y);

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

    public void FillFieldAfterFlow()
    {
        for (var i = 0; i < IcwGame.SizeX; i++)
            for (var j = 0; j < IcwGame.SizeY; j++)
            {   
                if (_field[i * IcwGame.SizeY + j].TileType != TileType.Empty)
                    _tmpFieldProjection[i, j] = 2;
                else _tmpFieldProjection[i, j] = 0;
            }
        
        // get area for each enemy 
        _startFillPoints.Clear();
        
        for (var i = 0; i < Enemies.transform.childCount; i++)
        {
            var startPos = Vector3Int.RoundToInt(Enemies.transform.GetChild(i).transform.position);
            _startFillPoints.Add(startPos);
        }
        
        while (_startFillPoints.Count > 0)
        {
            var currFillPoint = _startFillPoints[^1];
            _startFillPoints.RemoveAt(_startFillPoints.Count - 1);
            FillFromPoint(currFillPoint, 1);
        }
        
        // fills areas where enemy not detected
        for (var i = 0; i < IcwGame.SizeX; i++)
        for (var j = 0; j < IcwGame.SizeY; j++)
        {
            if (_tmpFieldProjection[i, j] == 0 || GetTileType(i ,j) == TileType.Trace)
                PutTile(TileType.Filled, i, j);
        }
    }
    
    #endregion

    public void HitTraceTile(Vector2Int pos)
        => HitTraceTile(pos.x, pos.y); 

    public void HitTraceTile(int x, int y)
        => _field[x * IcwGame.SizeY + y].GameObject.GetComponent<TraceTile>().HitByEnemy(this); 

    public bool HasTraceTiles()
        => _field.Any(f => f.TileType == TileType.Trace);
}
