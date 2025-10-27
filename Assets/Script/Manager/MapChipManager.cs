using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapChipManager : SingletonMonoBehaviour<MapChipManager>
{
    [SerializeField] private List<MapChipObj> _mapChipObjs = new List<MapChipObj>();
    [SerializeField] private int _mapSizeX = 4;
    [SerializeField] private int _mapSizeZ = 4;
    [SerializeField] private int _mapChipSize = 4;
    [SerializeField] private int _emptyId = 0;
    private bool _isMove = false;
    private bool _isFinishInit = false;
    private int _startId;
    private DirectionType _startDirection;
    private MapChipObj _endMCO;
    private GoalFlagObj _goalFlagObj;
    /// <summary>
    /// ステージ初期化
    /// </summary>
    /// <param name="mapData"></param>
    /// <returns></returns>
    public async UniTask StageInit(MapData mapData) 
    {
        _isFinishInit = false;
        _mapSizeX = 0;
        _mapSizeZ = 0;
        //MapSizeTypeによってマップサイズを設定
        switch (mapData.Type) 
        {
            case MapSizeType.FourByFour:
                _mapSizeX = 4;
                _mapSizeZ = 4;
                break;
            case MapSizeType.SixBySix:
                _mapSizeX = 6;
                _mapSizeZ = 6;
                break;
            case MapSizeType.EightByEight:
                _mapSizeX = 8;
                _mapSizeZ = 8;
                break;
        }
        int id = 0;
        
        //マップチップの生成・ID割り当て・アニメーション再生
        for (int x = 0; x < mapData.WrapperRoadTypes.Count; x++)
        {
            for (int y = 0; y < mapData.WrapperRoadTypes[x].RoadTypes.Count; y++)
            {
                RoadType type = mapData.WrapperRoadTypes[x].RoadTypes[y];
                MapChipObj mapChipObj = MapChipObjectPool.Instance.GetMapChipObj(type);
                mapChipObj.Init(GetPositionFromId(id), id);
                mapChipObj.Active();
                mapChipObj.SpawnAsync().Forget();
                _mapChipObjs.Add(mapChipObj);
                await UniTask.Yield();
                id++;
            }
        }
        _startId = mapData.StartId;
        _startDirection = mapData.StartDirection;
        _endMCO = GetMapChipObj(mapData.EndId);
        _emptyId = mapData.EndId;
    }
    /// <summary>
    /// MapChipの座標からIDを生成
    /// </summary>
    /// <param name="mapChipObj"></param>
    /// <returns></returns>
    public int GetId(MapChipObj mapChipObj)
    {
        int startX = 2 - _mapSizeX * 2;
        int startZ = _mapSizeZ * 2 - 2;
        int xIndex = ((int)mapChipObj.transform.position.x - startX) / _mapChipSize;
        int zIndex = (startZ - (int)mapChipObj.transform.position.z) / _mapChipSize;
        _mapChipObjs.Add(mapChipObj);
        return zIndex * _mapSizeZ + xIndex;
    }
    /// <summary>
    /// IDから座標を生成
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Vector3 GetPositionFromId(int id)
    {
        float xIndex = id % _mapSizeX;
        float zIndex = id - xIndex;

        float x = xIndex * _mapChipSize - 2f * (_mapSizeX - 1f);
        float z = -((float)_mapChipSize /(float) _mapSizeZ * (float)zIndex) + 2f * (_mapSizeZ - 1);

        return new Vector3(x, 0f, z);
    }
    /// <summary>
    /// MapChip移動処理
    /// </summary>
    /// <param name="mapChipOb"></param>
    public async void MapChipMovable(MapChipObj mapChipOb)
    {
        //ステージが初期化されるまで実行されない
        if (_isFinishInit == false) return;
        //移動中なら実行されない
        if (_isMove) return;
        _isMove = true;
        var mcoIds = GetNextToIds(mapChipOb);
        if (mcoIds.Count == 0)
        {
            _isMove = false;
            return;
        }
        foreach (var id in mcoIds)
        {
            if (id == _emptyId)
            {
                if (_emptyId == _endMCO.Id)
                {
                    EndObjAlphaChange(0);
                }
                await MapChipMove(mapChipOb, GetPositionFromId(id));
                var tmp = mapChipOb.Id;
                if (tmp == _endMCO.Id)
                {
                    EndObjAlphaChange(0.2f);
                }
                mapChipOb.SetId(_emptyId);
                _emptyId = tmp;

                if (_mapChipObjs.Any(x => x.Id == _startId))
                {
                    if (!MapChipInfoUtil.ActiveDecision(GetMapChipObj(_startId).RoadType, _startDirection))
                    {
                        _isMove = false;
                        return;
                    }
                }
                if (_mapChipObjs.Count(x => x.Id == _endMCO.Id) == 1)
                {
                    _isMove = RouteCheckManager.Instance.RouteCheck(_startId, _endMCO);
                    return;
                }
                break;
            }
        }
        _isMove = false;
    }
    private async UniTask MapChipMove(MapChipObj mapChipObj, Vector3 target, float duration = 0.5f)
    {
        await mapChipObj.transform.DOMove(target, duration).AsyncWaitForCompletion();
    }
    /// <summary>
    /// 現在地と方向によってIDを取得
    /// </summary>
    /// <param name="mapChipObj"></param>
    /// <param name="directionType"></param>
    /// <returns></returns>
    public int? GetNextToId(MapChipObj mapChipObj, DirectionType directionType)
    {
        int id = mapChipObj.Id;
        //左端判定
        if (id % _mapSizeX != 0 && directionType == DirectionType.Left)
        {
            return mapChipObj.Id - 1;
        }
        //右端判定
        if (id % _mapSizeX != _mapSizeX - 1 && directionType == DirectionType.Right)
        {
            return mapChipObj.Id + 1;
        }
        //上底判定
        if (mapChipObj.Id >= _mapSizeX && directionType == DirectionType.Up)
        {
            return mapChipObj.Id - _mapSizeX;
        }
        //下底判定
        if (mapChipObj.Id < _mapSizeX * (_mapSizeZ - 1) && directionType == DirectionType.Down)
        {
            return mapChipObj.Id + _mapSizeZ;
        }
        return null;
    }
    /// <summary>
    /// 隣接するIDの取得
    /// </summary>
    /// <param name="mapChipObj"></param>
    /// <returns></returns>
    public List<int> GetNextToIds(MapChipObj mapChipObj)
    {
        List<int> ids = new List<int>();
        int id = mapChipObj.Id;
        //左端判定
        if (id % _mapSizeX != 0)
        {
            ids.Add(mapChipObj.Id - 1);
        }
        //右端判定
        if (id % _mapSizeX != _mapSizeX - 1)
        {
            ids.Add(mapChipObj.Id + 1);
        }
        //上底判定
        if (mapChipObj.Id >= _mapSizeX)
        {
            ids.Add(mapChipObj.Id - _mapSizeX);
        }
        //下底判定
        if (mapChipObj.Id < _mapSizeX * (_mapSizeZ - 1))
        {
            ids.Add(mapChipObj.Id + _mapSizeZ);
        }
        return ids;
    }
    /// <summary>
    /// IDからマップチップを取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MapChipObj GetMapChipObj(int id)
    {
        return _mapChipObjs.FirstOrDefault(x => x.Id == id);
    }
    /// <summary>
    /// 現在地と方向から隣接マップチップ取得
    /// </summary>
    /// <param name="from"></param>
    /// <param name="directionType"></param>
    /// <returns></returns>
    private MapChipObj GetMapChipObj(MapChipObj from, DirectionType directionType)
    {
        var idTmp = GetNextToId(from, directionType);
        if (idTmp == null) return null;
        return GetMapChipObj((int)idTmp);
    }
    /// <summary>
    /// 隣接するマップチップの道がつながっているか
    /// </summary>
    /// <param name="from"></param>
    /// <param name="directionType"></param>
    /// <returns></returns>
    public MapChipObj GetNextMapChipObj(MapChipObj from, DirectionType directionType)
    {
        var nextMCO = GetMapChipObj(from, directionType);
        if (nextMCO == null) return null;
        List<RoadType> roadTypes = MapChipInfoUtil.GetNotConectType(from.RoadType, directionType);
        if (roadTypes == null) return null;
        foreach (var notConectType in roadTypes)
        {
            if (notConectType == nextMCO.RoadType)
            {
                return null;
            }
        }
        return nextMCO;
    }
    /// <summary>
    /// 現在地と目的地のマップチップから方向を取得
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public DirectionType GetNextToDirection(MapChipObj from, MapChipObj to)
    {
        List<DirectionType> directionTypes = new List<DirectionType>()
        {
            DirectionType.Up,
            DirectionType.Left,
            DirectionType.Right,
            DirectionType.Down
        };
        foreach (var directionType in directionTypes)
        {
            if (GetNextMapChipObj(from, directionType) == to)
            {
                return directionType;
            }
        }
        return DirectionType.None;
    }
    /// <summary>
    /// ステージ初期化完了フラグ
    /// </summary>
    public void FinishInitFlag() 
    {
        _isFinishInit = true;
    }
    /// <summary>
    /// ゴール用の旗を表示
    /// </summary>
    /// <returns></returns>
    public async UniTask ViewGoalFlag()
    {
        _goalFlagObj = FlagObjectPool.Instance.GetGoalFlag();
        _goalFlagObj.Init(_endMCO.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == DirectionType.Center).Target.position);
        _goalFlagObj.Active();
        await _goalFlagObj.RaiseFlag();
        _endMCO.SetEnd();
        await UniTask.WaitForSeconds(1);
        EndObjAlphaChange(0.2f);
    }
    /// <summary>
    /// ゴールのマップチップのアルファ変更
    /// </summary>
    /// <param name="alpha"></param>
    public void EndObjAlphaChange(float alpha)
    {
        _endMCO.SetMaterialAlhpa(alpha);
        _goalFlagObj.SetMaterialAlhpa(alpha);
    }
}
