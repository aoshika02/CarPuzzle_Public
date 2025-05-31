using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteCheckManager : SingletonMonoBehaviour<RouteCheckManager>
{
    MapChipManager _mapChipManager;

    private void Start()
    {
        _mapChipManager = MapChipManager.Instance;
    }
    /// <summary>
    /// ルートが完成しているか
    /// </summary>
    /// <param name="startId"></param>
    /// <param name="endMCO"></param>
    /// <returns></returns>
    public bool RouteCheck(int startId, MapChipObj endMCO)
    {
        MapChipObj startMCO = _mapChipManager.GetMapChipObj(startId);
        if (startMCO == null || endMCO == null)
        {
            return false;
        }
        var route = MakeRoute(startMCO, endMCO);
        if (route != null)
        {
            GameFlow.Instance.SetRoute(route);
            return true;
        }
        else
        {
            Debug.Log("Faild");
        }
        return false;
    }
    /// <summary>
    /// ルート作成
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    private List<MapChipObj> MakeRoute(MapChipObj start, MapChipObj goal)
    {
        List<List<MapChipObj>> allRoutes = new List<List<MapChipObj>>();
        FindAllRoutes(start, goal, new List<MapChipObj>(), allRoutes);

        if (allRoutes.Count == 0) return null;
        //最短のルートを取得
        return allRoutes.OrderBy(route => route.Count).First();
    }
    /// <summary>
    /// 全ルートを再帰的に探索
    /// </summary>
    /// <param name="from">現在地</param>
    /// <param name="to">ゴール地点</param>
    /// <param name="currentRoute">現在のルート</param>
    /// <param name="allRoutes">全ルート</param>
    private void FindAllRoutes(MapChipObj from, MapChipObj to, List<MapChipObj> currentRoute, List<List<MapChipObj>> allRoutes)
    {
        //重複回避
        if (currentRoute.Contains(from)) return;

        currentRoute.Add(from);

        //ゴールに到達
        if (from == to)
        {
            allRoutes.Add(new List<MapChipObj>(currentRoute)); 
            currentRoute.RemoveAt(currentRoute.Count - 1); 
            return;
        }

        List<DirectionType> directions = MapChipInfoUtil.GetActiveDirectionType(from.RoadType);
        foreach (var dir in directions)
        {
            var nextObj = _mapChipManager.GetNextMapChipObj(from, dir);
            if (nextObj == null) continue;

            FindAllRoutes(nextObj, to, currentRoute, allRoutes);
        }

        currentRoute.RemoveAt(currentRoute.Count - 1); 
    }
}
