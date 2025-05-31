using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameFlow : SingletonMonoBehaviour<GameFlow>
{
    private List<MapChipObj> _mapChipObjs = null;
    [SerializeField] private MapDataScriptableObject _mapDataScriptableObject;
    
    /// <summary>
    /// ゲームフローのスタート
    /// </summary>
    /// <param name="mapSizeType"></param>
    public void StartGameFlow(MapSizeType mapSizeType) 
    {
        //マップサイズによって取得するデータ変更
        InGameFlow(
            _mapDataScriptableObject
            .MapDatas
            .FirstOrDefault(x => x.Type == mapSizeType))
            .Forget();
    }
    /// <summary>
    /// ゲームのフロー
    /// </summary>
    /// <param name="mapData"></param>
    /// <returns></returns>
    private async UniTask InGameFlow(MapData mapData)
    {
        if (mapData == null)
        {
            Debug.LogError($"MapDataにnullが引き渡されました");
            return;
        }
        //ブロック生成
        MapChipManager mapChipManager = MapChipManager.Instance;
        CarController carController=CarController.Instance;
        await mapChipManager.StageInit(mapData);
        await UniTask.WaitForSeconds(1);
        //車出現
        carController.Init(mapChipManager
            .GetMapChipObj(mapData.StartId)
            .MapChipData
            .TargetPosDatas
            .FirstOrDefault(x => x.DirectionType == mapData.StartDirection)
            .Target
            .position, 
            mapData.StartDirection);
        carController.Active();
        await carController.SpawnAsync();
        await UniTask.WaitForSeconds(1.5f);
        //ゴール旗生成
        await mapChipManager.ViewGoalFlag();
        mapChipManager.FinishInitFlag();
        //ルート完成まで待機
        await UniTask.WaitUntil(() => _mapChipObjs != null);
        //ルート完成後車が動く
        mapChipManager.EndObjAlphaChange(1);
        await CarController.Instance.MoveOnRoute(_mapChipObjs);
        await UniTask.WaitForSeconds(1f);
        //スコアの表示
        await FlagCountManager.Instance.ViewFlagCount();
        //タイトルに戻るボタン出現
    }
    /// <summary>
    /// ルート取得用
    /// </summary>
    /// <param name="mapChipObjs"></param>
    public void SetRoute(List<MapChipObj> mapChipObjs) 
    {
        _mapChipObjs = mapChipObjs;
    }
}

