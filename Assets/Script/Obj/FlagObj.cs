using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FlagObj : MonoBehaviour
{
    public bool IsUse = false;
    public void Init(Vector3 newPos)
    {
        transform.localScale = Vector3.zero;
        transform.position = newPos;
    }
    /// <summary>
    /// 旗を立てるアニメーション
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="isNomalFlag"></param>
    /// <returns></returns>
    public async UniTask RaiseFlag(float duration = 0.5f,bool isNomalFlag =true) 
    {
        await transform.DOScale(new Vector3(2, 2, 2), duration);
        await transform.DOScale(new Vector3(1,1,1), duration);
        if (isNomalFlag == false) return;
        FlagCountManager.Instance.AddFlagCount();
    }
    /// <summary>
    /// 有効化
    /// </summary>
    public void Active() 
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 無効化
    /// </summary>
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}