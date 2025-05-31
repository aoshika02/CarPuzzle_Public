using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FadeInout : SingletonMonoBehaviour<FadeInout>
{
    [SerializeField] private CanvasGroup _canvasGroup;
    protected override void Awake()
    {
        if (!CheckInstance())
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(float duration=0.5f) 
    {
        await DOVirtual.Float(1, 0, duration, f =>
        {
            _canvasGroup.alpha = f;
        }).ToUniTask();
    }
    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(float duration = 0.5f)
    {
        await DOVirtual.Float(0, 1, duration, f =>
        {
            _canvasGroup.alpha = f;
        }).ToUniTask();
    }
}
