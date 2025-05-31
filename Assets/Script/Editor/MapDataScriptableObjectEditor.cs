#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge.Xml;
using UnityEditor.PackageManager.UI;

[CustomEditor(typeof(MapDataScriptableObject))]
public class MapDataScriptableObjectEditor : Editor
{
    private List<bool> _isOpenDatas = new List<bool>();
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MapDataScriptableObject MapDataSO = (MapDataScriptableObject)target;

        if (MapDataSO.MapDatas == null)
            MapDataSO.MapDatas = new List<MapData>();
        List<MapData> mapDatas = MapDataSO.MapDatas;
        for (int i = 0; i < mapDatas.Count; i++)
        {
            _isOpenDatas.Add(false);
            _isOpenDatas[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_isOpenDatas[i], $"Spawn{nameof(MapData)} : {i + 1}");
            if (_isOpenDatas[i])
            {
                mapDatas[i].Type = (MapSizeType)EditorGUILayout.EnumPopup(MapDataSO.MapDatas[i].Type, GUILayout.Width(80));

                int mapSize = 0;
                switch (mapDatas[i].Type)
                {

                    case MapSizeType.FourByFour:
                        mapSize = 4;
                        break;
                    case MapSizeType.SixBySix:
                        mapSize = 6;
                        break;
                    case MapSizeType.EightByEight:
                        mapSize = 8;
                        break;
                }
                if (mapSize == 0) return;
                if (mapDatas[i].WrapperRoadTypes == null)
                    mapDatas[i].WrapperRoadTypes = new List<RoadTypeWrapper>();
                for (int x = 0; x < mapSize; x++)
                {
                    // 行が足りない場合追加
                    if (mapDatas[i].WrapperRoadTypes.Count <= x)
                        mapDatas[i].WrapperRoadTypes.Add(new RoadTypeWrapper());
                    EditorGUILayout.BeginHorizontal();
                    for (int y = 0; y < mapSize; y++)
                    {
                        // 列が足りない場合追加
                        if (mapDatas[i].WrapperRoadTypes[x].RoadTypes.Count <= y)
                            mapDatas[i].WrapperRoadTypes[x].RoadTypes.Add(RoadType.None);

                        mapDatas[i].WrapperRoadTypes[x].RoadTypes[y] = (RoadType)EditorGUILayout.EnumPopup(mapDatas[i].WrapperRoadTypes[x].RoadTypes[y], GUILayout.Width(80));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                mapDatas[i].StartId = EditorGUILayout.IntSlider($"{nameof(MapData.StartId)}", mapDatas[i].StartId, 0, mapSize * mapSize - 1);
                mapDatas[i].StartDirection = (DirectionType)EditorGUILayout.EnumPopup($"{nameof(MapData.StartDirection)}", mapDatas[i].StartDirection);
                mapDatas[i].EndId = EditorGUILayout.IntSlider($"{nameof(MapData.EndId)}", mapDatas[i].EndId, 0, mapSize * mapSize - 1);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (GUILayout.Button($"Add {nameof(MapData)}"))
        {
            MapDataSO.MapDatas.Add(new MapData());
        }

        if (GUILayout.Button($"Remove Last {nameof(MapData)}") && MapDataSO.MapDatas.Count > 0)
        {
            MapDataSO.MapDatas.RemoveAt(MapDataSO.MapDatas.Count - 1);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(MapDataSO);
        }
    }
}
#endif