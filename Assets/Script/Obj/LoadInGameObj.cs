using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class LoadInGameObj : MonoBehaviour
{
    [SerializeField] private MapSizeType _mapSizeType;
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
        //ゲーム本編を読み込み
        SceneLoadManager.Instance.LoadSceneAsync(SceneType.InGame, _mapSizeType).Forget();
    }
}
