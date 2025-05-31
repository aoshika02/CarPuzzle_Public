using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataScriptableObject", menuName = "ScriptableObjects/MapDataScriptableObject")]
public class MapDataScriptableObject : ScriptableObject
{
    public List<MapData> MapDatas; 
}

[Serializable]
public class MapData 
{
    public MapSizeType Type;
    public List<RoadTypeWrapper> WrapperRoadTypes;
    public int StartId;
    public DirectionType StartDirection;
    public int EndId;
}
[Serializable]
public class RoadTypeWrapper
{
    public List<RoadType> RoadTypes = new List<RoadType>();
}
public enum MapSizeType
{
    FourByFour,
    SixBySix,
    EightByEight
}