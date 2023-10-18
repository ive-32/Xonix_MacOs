using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class Field : MonoBehaviour
{
    [FormerlySerializedAs("TilesList")] public List<GameObject> tilesList = new List<GameObject>();
    [NonSerialized] public Enemies Enemies;
    [NonSerialized] public FieldMeta FieldMeta;
    [NonSerialized] public GameObject SplashTextPrefab;
    
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
        LoadField();
    }

    public TileType GetTileType(int x, int y)
        => _field[x * IcwGame.SizeY + y].TileType;

    public TileType GetTileType(Vector2Int pos)
        => _field[pos.x * IcwGame.SizeY + pos.y].TileType;

    public TileType GetTileType(Vector3 pos)
        => _field[Mathf.RoundToInt(pos.x) * IcwGame.SizeY + Mathf.RoundToInt(pos.y)].TileType;

    public FieldTile GetTile(Vector2Int pos)
        => _field[pos.x * IcwGame.SizeY + pos.y];

    public FieldTile GetTileWithValidation(Vector2Int pos)
        => pos.IsPositionValid() ? _field[pos.x * IcwGame.SizeY + pos.y] : null;
    
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
        
        for (var i = 0; i < 4; i++)
        {
            tmpVector = Quaternion.AngleAxis(90, Vector3.forward) * tmpVector;
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

        var scores = 0;
        var centerTile = Vector2.zero;
        // fills areas where enemy not detected
        for (var i = 0; i < IcwGame.SizeX; i++)
        for (var j = 0; j < IcwGame.SizeY; j++)
        {
            if (_tmpFieldProjection[i, j] == 0 || GetTileType(i, j) == TileType.Trace)
            {
                PutTile(TileType.Filled, i, j);
                centerTile += new Vector2(i, j);
                scores++;
            }
        }

        centerTile = scores == 0 ? centerTile : centerTile / scores;
        if (centerTile != Vector2.zero && SplashTextPrefab is not null)
        {
            var splashText = Instantiate(SplashTextPrefab, centerTile, quaternion.identity);
            splashText.GetComponent<UiSplashLabel>().SetText($"+{scores}");
        }
        
        IcwGame.Scores += scores;
    }
    
    #endregion

    public void HitTraceTile(Vector2Int pos)
        => HitTraceTile(pos.x, pos.y); 

    public void HitTraceTile(int x, int y)
        => _field[x * IcwGame.SizeY + y].GameObject.GetComponent<TraceTile>().HitByEnemy(this); 

    public bool HasTraceTiles()
        => _field.Any(f => f.TileType == TileType.Trace);

    public void ClearTraceTiles()
    {
        for (var i = 0; i < IcwGame.SizeX; i++)
            for (var j = 0; j < IcwGame.SizeY; j++)
                if (_field[i * IcwGame.SizeY + j].TileType == TileType.Trace)
                    PutTile(TileType.Empty, i, j);
    }
    
    public int GetFillPercents()
    {
        var total = (IcwGame.SizeX - 4) * (IcwGame.SizeY - 4);
        var borders = IcwGame.SizeX * 4 + IcwGame.SizeY * 4 - 16;
        var filled = _field.Count(t => t.TileType.IsGround()) - borders;
        return filled * 100 / total;
    }

    private void LoadField()
    {
        var targetFile = Resources.Load<TextAsset>($"Level{IcwGame.Level}");

        if (targetFile is null)
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
            return;
        }
        
        FieldMeta = JsonConvert.DeserializeObject<FieldMeta>(targetFile.ToString());
        
        for (var y = 0; y < IcwGame.SizeY; y++)
        {
            for (var x = 0; x < IcwGame.SizeX; x++)
            {
                var tile = Enum.Parse<TileType>(FieldMeta.Field[y][x].ToString());
                PutTile(tile, x, IcwGame.SizeY - 1 - y);
            }
        }
    }

    private void SaveField()
    {
        var storedField = new FieldMeta();

        var s = new char[IcwGame.SizeX];
        for (var y = 0; y < IcwGame.SizeY; y++)
        {
            Span<char> target = s;
            for (var x = 0; x < IcwGame.SizeX; x++)
                target[x] = ((int)_field[x * IcwGame.SizeY + y].TileType).ToString()[0];
            storedField.Field[y] = target.ToString();
        }
        var serialisedField = JsonConvert.SerializeObject(storedField);
        
        File.WriteAllText(Application.persistentDataPath + "/Level.json", serialisedField);         

    }
}
