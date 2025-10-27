using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private List<Canvas> _hitCanvases;
    public IObservable<List<GameObject>> TappedAsObservable => _onTapped;
    private readonly Subject<List<GameObject>> _onTapped = new Subject<List<GameObject>>();

    public IObservable<GameObject> OnTappedUI => _onTappedUI;
    private readonly Subject<GameObject> _onTappedUI = new Subject<GameObject>();
    public IReadOnlyReactiveProperty<GameObject> OnOveredUI => _onOveredUI;
    private readonly ReactiveProperty<GameObject> _onOveredUI = new ReactiveProperty<GameObject>();

    private List<GraphicRaycaster> _raycasters = new List<GraphicRaycaster>();
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    protected override void Awake()
    {
        if (!CheckInstance())
        {
            return;
        }
        _playerAction = new PlayerAction();

        foreach (var canvas in _hitCanvases)
        {
            _raycasters.Add(canvas.GetComponent<GraphicRaycaster>());
        }
        _eventSystem = EventSystem.current;
        OnEnable();
    }
    /// <summary>
    /// 有効化
    /// </summary>
    private void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.PlayerTouch.Tap.canceled += OnTapCanceled;
    }
    /// <summary>
    /// 無効化
    /// </summary>
    private void OnDisable()
    {
        _playerAction.PlayerTouch.Tap.canceled -= OnTapCanceled;
        _playerAction.Disable();
    }
    private void Update()
    {
        UITapEvent(out var outObj);
        _onOveredUI.Value = outObj;
    }
    /// <summary>
    /// タップ関連
    /// </summary>
    /// <param name="context"></param>
    private void OnTapCanceled(InputAction.CallbackContext context)
    {
        //UI優先でタップ
        if (UITapEvent(out var outObj))
        {
            _onTappedUI.OnNext(outObj);
            return;
        }
        ObjTapEvent();
    }
    #region 3DObj
    private void ObjTapEvent()
    {
        var tmpScreenPos = GetDeviceValue();
        if (tmpScreenPos == null) return;
        Vector3 screenPos = tmpScreenPos.Value;
        screenPos.z = _mainCamera.nearClipPlane;

        //UnityのWorldPositionに置き換え
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);

        Vector3 direction = (worldPos - _mainCamera.transform.position).normalized;

        //オブジェクトの取得
        List<GameObject> hitObjs = RaycastSpecificHit(worldPos, direction, "MapChipObj");

        if (hitObjs != null)
        {
            _onTapped.OnNext(hitObjs);
        }
    }
    /// <summary>
    /// Raycastを使い、条件に合うGameObjectを返す
    /// </summary>
    /// <param name="origin">Raycastの開始地点</param>
    /// <param name="direction">Raycastの向き</param>
    /// <param name="targetTag">目的のタグ</param>
    /// <param name="targetLayer">目的のレイヤー</param>
    /// <param name="distance">Raycastの長さ</param>
    /// <returns></returns>
    private List<GameObject> RaycastSpecificHit(
        Vector3 origin,
        Vector3 direction,
        string targetTag = null,
        int targetLayer = -1,
        float distance = Mathf.Infinity)
    {
        //nullフラグ
        bool isTagNull = string.IsNullOrEmpty(targetTag);
        bool isLayerNull = targetLayer < 0;
        //ターゲットのTagとLayer番号が指定されていない場合中断
        if (isTagNull && isLayerNull) return null;
        RaycastHit[] hitObjs = Physics.RaycastAll(origin, direction, distance);
        Debug.DrawRay(origin, direction, Color.red, 1f);
        if (hitObjs == null || hitObjs.Length == 0) return null;
        RaycastHit[] sortedHits = hitObjs.OrderBy(hit => hit.distance).ToArray();
        List<GameObject> hisObjs = new List<GameObject>();
        foreach (var hitObj in sortedHits)
        {
            if (hitObj.collider == null) continue;
            //tagがnullかタグが合致する
            bool isTagMatch = isTagNull || hitObj.collider.tag == targetTag;
            //layerがnullが合致する
            bool isLayerMatch = isLayerNull || hitObj.collider.gameObject.layer == targetLayer;
            //タグかレイヤーがマッチしたオブジェクトを返す
            if (isTagMatch && isLayerMatch)
            {
                hisObjs.Add(hitObj.collider.gameObject);
            }
        }
        return hisObjs;
    }
    #endregion

    private bool UITapEvent(out GameObject outObj)
    {
        var tmpScreenPos = GetDeviceValue();
        outObj = null;
        if (tmpScreenPos == null) return false;
        Vector3 screenPos = tmpScreenPos.Value;
        screenPos.z = _mainCamera.nearClipPlane;

        List<GameObject> uiHits = RaycastUI(screenPos);
        if (uiHits != null && uiHits.Count > 0)
        {
            outObj = uiHits[0];
            return true;
        }
        return false;
    }
    private List<GameObject> RaycastUI(Vector2 screenPosition)
    {
        _pointerEventData = new PointerEventData(_eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        List<List<RaycastResult>> resultDatas = new List<List<RaycastResult>>();
        foreach (var cast in _raycasters)
        {
            cast.Raycast(_pointerEventData, results);
            resultDatas.Add(new List<RaycastResult>(results));
            results.Clear();
        }

        return resultDatas.SelectMany(list => list).Select(r => r.gameObject).ToList();
    }
    /// <summary>
    /// デバイスに応じた座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3? GetDeviceValue()
    {
        //マウスが接続ならマウス座標を返す
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        //指の座標を返す
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.touches.Count == 1)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }
        }
        //例外
        return null;
    }
}
