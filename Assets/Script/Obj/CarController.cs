using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : SingletonMonoBehaviour<CarController>
{
    MapChipManager _mapChipManager;
    [Serializable]
    public class CarForwardData
    {
        public DirectionType DirectionType;
        public float Angle;
    }
    [SerializeField] private List<CarForwardData> _carForwards = new List<CarForwardData>();
    private void Start()
    {
        _mapChipManager = MapChipManager.Instance;
        Deactive();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="startDirection"></param>
    public void Init(Vector3 pos,DirectionType startDirection)
    {
        gameObject.transform.position = pos;
        transform.localScale = Vector3.zero;
        float startAngle = 0;
        switch (startDirection)
        {
            case DirectionType.Up:
                startAngle = _carForwards.FirstOrDefault(x => x.DirectionType == DirectionType.Down).Angle;
                break;
            case DirectionType.Left:
                startAngle = _carForwards.FirstOrDefault(x => x.DirectionType == DirectionType.Right).Angle;
                break;
            case DirectionType.Right:
                startAngle = _carForwards.FirstOrDefault(x => x.DirectionType == DirectionType.Left).Angle;
                break;
            case DirectionType.Down:
                startAngle = _carForwards.FirstOrDefault(x => x.DirectionType == DirectionType.Up).Angle;
                break;
        }
        transform.rotation = Quaternion.Euler(0, startAngle, 0);
    }
    /// <summary>
    /// スポーンアニメーション
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask SpawnAsync(float duration = 0.5f)
    {
        await transform.DOScale(new Vector3(2, 2, 2), duration);
        await transform.DOScale(new Vector3(1, 1, 1), duration);
    }
    /// <summary>
    /// ルートに沿って移動
    /// </summary>
    /// <param name="mapChipObjs"></param>
    /// <returns></returns>
    public async UniTask MoveOnRoute(List<MapChipObj> mapChipObjs)
    {
        DirectionType nextDirection;
        for (int i = 1; i < mapChipObjs.Count; i++)
        {
            nextDirection = _mapChipManager.GetNextToDirection(mapChipObjs[i - 1], mapChipObjs[i]);
            await SelectMovement(mapChipObjs[i - 1], nextDirection);

            //通った道に旗を立てる
            FlagObj flagObj = FlagObjectPool.Instance.GetFlagObj();
            Vector3 flagPos = mapChipObjs[i].MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == DirectionType.Center).Target.position;
            if (flagPos == null) continue;
            flagObj.Init(flagPos);
            flagObj.Active();
            flagObj.RaiseFlag().Forget();

            //ルートの最後なら最終地点に移動
            if (i == mapChipObjs.Count - 1)
            {
                await SelectMovement(mapChipObjs[i], DirectionType.Center);
            }
        }
    }
    /// <summary>
    /// 移動方法を選択
    /// </summary>
    /// <param name="mapChipObj"></param>
    /// <param name="nextDirection"></param>
    /// <returns></returns>
    private UniTask SelectMovement(MapChipObj mapChipObj, DirectionType nextDirection)
    {
        //道の属性によって移動方法を変更
        switch (mapChipObj.RoadType)
        {
            case RoadType.UpDown:
            case RoadType.LeftRight:
                return LinearMove(mapChipObj.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == nextDirection).Target, 0.7f);
            case RoadType.UpLeft:
            case RoadType.UpRight:
            case RoadType.LeftDown:
            case RoadType.RightDown:
                return CurveMove(
                    mapChipObj.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == DirectionType.Center).Target,
                    mapChipObj.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == nextDirection).Target,
                    nextDirection,
                    0.7f);
            case RoadType.TUp:
            case RoadType.TDown:
            case RoadType.TLeft:
            case RoadType.TRight:
                return RightAngleMove(
                    mapChipObj.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == DirectionType.Center).Target,
                    mapChipObj.MapChipData.TargetPosDatas.FirstOrDefault(x => x.DirectionType == nextDirection).Target,
                    0.7f);
        }
        return UniTask.CompletedTask;
    }
    /// <summary>
    /// 直線移動
    /// </summary>
    /// <param name="endPos">目的地</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    public async UniTask LinearMove(Transform endPos, float duration)
    {
        await transform.DOMove(endPos.position, duration).AsyncWaitForCompletion();
    }
    /// <summary>
    /// カーブ移動
    /// </summary>
    /// <param name="mediatePos">中継地</param>
    /// <param name="endPos">目的地</param>
    /// <param name="nextDirection">向かう方向</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    public async UniTask CurveMove(Transform mediatePos, Transform endPos,DirectionType nextDirection, float duration)
    {
        List <UniTask> tasks = new List<UniTask>();
        Transform startPos = transform;
        Vector3[] targetData = new Vector3[] {
            startPos.position,
            mediatePos.position,
            endPos.position
        };
        //カーブ移動
        tasks.Add(transform.DOPath(targetData,duration).SetOptions(false).ToUniTask());
        if( nextDirection==DirectionType.Up ||
            nextDirection == DirectionType.Left || 
            nextDirection == DirectionType.Right||
            nextDirection == DirectionType.Down) 
        {
            float targetAngle = _carForwards.FirstOrDefault(x => x.DirectionType == nextDirection).Angle;
            //向かう方向へと回転
            tasks.Add(transform.DORotate(new Vector3(0, targetAngle, 0), duration, RotateMode.Fast).ToUniTask());
        }
        
        await UniTask.WhenAll(tasks);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediatePos">中継地</param>
    /// <param name="endPos">目的地</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    public async UniTask RightAngleMove(Transform mediatePos, Transform endPos, float duration)
    {
        await transform.DOMove(mediatePos.position, duration * 2f / 5f).AsyncWaitForCompletion();
        await transform.DOLookAt(endPos.position, duration / 5f).AsyncWaitForCompletion();
        await transform.DOMove(endPos.position, duration * 2f / 5f).AsyncWaitForCompletion();
    }
   
    public void Active()
    {
        gameObject.SetActive(true);
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}
