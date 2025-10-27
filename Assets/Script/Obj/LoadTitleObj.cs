using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class LoadTitleObj : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.OnTappedUI
            .Where(obj => obj != null)
            .Where(obj => obj == gameObject)
            .Subscribe(_ => OnTapped())
            .AddTo(this);
    }

    private void OnTapped()
    {
        //タイトル読み込み
        SceneLoadManager.Instance.LoadSceneAsync(SceneType.Title).Forget();
    }
}
