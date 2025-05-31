using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class MapChipObj : MonoBehaviour
{
    [SerializeField] private MapChipData _mapChipData;
    [SerializeField] private AlphaChange _alphaChange;
    public MapChipData MapChipData => _mapChipData;
    [SerializeField] RoadType _roadType;
    public RoadType RoadType => _roadType;

    [SerializeField] private int _id;
    public int Id => _id;
    MapChipManager _mapChipManager;
    [SerializeField] private bool _isMovable = true;
    private void Start()
    {
        _mapChipManager = MapChipManager.Instance;
        InputManager.Instance.TappedAsObservable
            .Where(obj => obj != null)
            .Where(obj => obj.Contains(gameObject))
            .Subscribe(_ => OnTapped())
            .AddTo(this);
    }
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    public void Init(Vector3 pos, int id)
    {
        gameObject.transform.position = pos;
        SetId(id);
        transform.localScale = Vector3.zero;
    }
    /// <summary>
    /// タップ時の処理
    /// </summary>
    private void OnTapped()
    {
        if (_isMovable == false) return;
        _mapChipManager.MapChipMovable(this);
    }
    /// <summary>
    /// IDのセット
    /// </summary>
    public void SetId()
    {
        _id = _mapChipManager.GetId(this);
    }
    /// <summary>
    /// IDのセット
    /// </summary>
    /// <param name="id">セットするID</param>
    public void SetId(int id)
    {
        _id = id;
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
    /// アルファ変更
    /// </summary>
    /// <param name="alpha"></param>
    public void SetMaterialAlhpa(float alpha)
    {
        _alphaChange.SetAlpha(alpha);
    }
    /// <summary>
    ///ゴール地点設定
    /// </summary>
    public void SetEnd()
    {
        _isMovable = false;
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
