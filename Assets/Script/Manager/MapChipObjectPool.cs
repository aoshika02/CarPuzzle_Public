using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapChipObjectPool : SingletonMonoBehaviour<MapChipObjectPool>
{
    [Serializable]
    public class MapChipCreationData
    {
        public RoadType RoadType;
        public GameObject MapChipObj;
    }
    [SerializeField] private List<MapChipCreationData> _mapChipSpawnDatas;
    public class InstanceMapChipData
    {
        public MapChipObj MapChipObj;
        public bool IsUse;
    }
    private List<InstanceMapChipData> _mapChipObjs = new List<InstanceMapChipData>();

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
    }
    /// <summary>
    /// マップチップの取得
    /// </summary>
    /// <param name="roadType"></param>
    /// <returns></returns>
    public MapChipObj GetMapChipObj(RoadType roadType) 
    {
        MapChipObj mapChipObj = null;
        foreach (var mco in _mapChipObjs)
        {
            if (mco.IsUse == true) continue;
            mapChipObj = mco.MapChipObj;
            mco.IsUse = true;
        }
        if (mapChipObj == null) 
        {
            mapChipObj = CreateMapChipObj(roadType);
        }
        return mapChipObj;
    }
    /// <summary>
    /// マップチップ生成
    /// </summary>
    /// <param name="roadType"></param>
    /// <returns></returns>
    public MapChipObj CreateMapChipObj(RoadType roadType) 
    {
        GameObject obj = Instantiate(_mapChipSpawnDatas.FirstOrDefault(x => x.RoadType == roadType).MapChipObj, transform);
        MapChipObj mapChipObj = obj.GetComponent<MapChipObj>();
        mapChipObj.Deactive();
        _mapChipObjs.Add(
            new InstanceMapChipData
            {
                MapChipObj = mapChipObj,
                IsUse = true
            });
        return mapChipObj;
    }
    /// <summary>
    /// マップチップの変換
    /// </summary>
    /// <param name="mapChipObj"></param>
    public void ReleaseInstanceMapChip (MapChipObj mapChipObj)
    {
        _mapChipObjs.FirstOrDefault(x => x.MapChipObj == mapChipObj).IsUse = false;
        mapChipObj.Deactive();
    }
}
