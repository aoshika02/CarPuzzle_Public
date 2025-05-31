using System.Threading;
public class CancelToken
{
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    /// <summary>
    /// クラス作成時
    /// </summary>
    public CancelToken()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    /// <summary>
    /// トークン取得
    /// </summary>
    /// <returns></returns>
    public CancellationToken GetToken()
    {
        return _cancellationToken;
    }

    /// <summary>
    /// キャンセル処理
    /// </summary>
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 再生成
    /// </summary>
    public void ReCreate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }
}