using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    private bool _isLoad = false;
    protected override void Awake()
    {
        if (!CheckInstance())
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// シーンの読み込み
    /// </summary>
    /// <param name="sceneType">読み込むシーン</param>
    /// <param name="mapSizeType">読み込みたいマップのサイズ</param>
    /// <returns></returns>
    public async UniTask LoadSceneAsync(SceneType sceneType, MapSizeType mapSizeType = MapSizeType.FourByFour)
    {
        if (_isLoad) return;
        _isLoad = true;
        await FadeInout.Instance.FadeOut();
        //enumの数値からシーン読み込み
        await SceneManager.LoadSceneAsync((int)sceneType);
        await UniTask.Yield();
        //InGameを読み込むときのみゲームフローの開始処理
        if (sceneType == SceneType.InGame)
        {
            GameFlow.Instance.StartGameFlow(mapSizeType);
        }
        await FadeInout.Instance.FadeIn();
        _isLoad = false;
    }
}
public enum SceneType
{
    Title = 0,
    InGame = 1
}
