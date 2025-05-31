using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class LoadTitleObj : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.TappedAsObservable
            .Where(obj => obj != null)
            .Where(obj => obj.Contains(gameObject))
            .Subscribe(_ => OnTapped())
            .AddTo(this);
    }

    private void OnTapped()
    {
        //タイトル読み込み
        SceneLoadManager.Instance.LoadSceneAsync(SceneType.Title).Forget();
    }
}
