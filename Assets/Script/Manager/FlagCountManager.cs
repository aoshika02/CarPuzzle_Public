using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FlagCountManager : SingletonMonoBehaviour<FlagCountManager>
{
    private int _flagCount = 0;
    [SerializeField] private CanvasGroup _flagCountCanvasGroup;
    [SerializeField] private CanvasGroup _ToTitleCanvasGroup;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private GameObject _toTitleObj;

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        Deactive();
    }
    /// <summary>
    /// 立てた旗の合計を表示
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask ViewFlagCount(float duration = 1f) 
    {
        _flagCountCanvasGroup.alpha = 1;
        int flagCount = 0;
        float time = 0;
        while(time<duration)
        {
            time += Time.deltaTime;
            flagCount=Random.Range(0, 50);
            _textMeshProUGUI.text = $"立てた旗の数 ：{flagCount}";
            await UniTask.Yield();
        }
        _textMeshProUGUI.text = $"立てた旗の数 ：{_flagCount}";
        await UniTask.WaitForSeconds(0.5f);
        _ToTitleCanvasGroup.alpha = 1;
        _toTitleObj.SetActive(true);
    }
    /// <summary>
    /// 旗加算
    /// </summary>
    public void AddFlagCount()
    {
        _flagCount++; 
    }
    /// <summary>
    /// 非アクティブ処理
    /// </summary>
    public void Deactive()
    {
        _flagCountCanvasGroup.alpha = 0;
        _ToTitleCanvasGroup.alpha = 0;
        _toTitleObj.SetActive(false);
    }
}
