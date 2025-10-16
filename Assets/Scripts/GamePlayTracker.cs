using UnityEngine;

/// <summary>
/// ゲームプレイ中の統計を記録するトラッカー
/// ゲームシーンに配置して、プレイ時間などを自動記録
/// </summary>
public class GamePlayTracker : MonoBehaviour
{
    private float sessionStartTime;
    private bool isTracking = false;
    private bool hasRecordedStart = false;
    
    [Header("Auto Save Settings")]
    [SerializeField] private float autoSaveInterval = 10f; // 10秒ごとに自動保存
    private float nextAutoSaveTime;

    void Awake()
    {
        // ゲームスタートを記録（Awakeで確実に実行）
        if (!hasRecordedStart)
        {
            GameStatistics.RecordGameStart();
            hasRecordedStart = true;
            Debug.Log("[GamePlayTracker] ゲームスタート記録完了");
        }
    }

    void Start()
    {
        // セッション開始時間を記録（realtimeSinceStartupを使用）
        sessionStartTime = Time.realtimeSinceStartup;
        isTracking = true;
        nextAutoSaveTime = Time.realtimeSinceStartup + autoSaveInterval;
        
        Debug.Log("[GamePlayTracker] プレイセッション開始");
    }

    void Update()
    {
        // 定期的にプレイ時間を保存
        if (isTracking && Time.realtimeSinceStartup >= nextAutoSaveTime)
        {
            RecordSessionPlayTime();
            nextAutoSaveTime = Time.realtimeSinceStartup + autoSaveInterval;
        }
    }

    void OnDestroy()
    {
        // シーン終了時にプレイ時間を記録
        RecordSessionPlayTime();
    }

    void OnApplicationQuit()
    {
        // アプリ終了時にもプレイ時間を記録
        RecordSessionPlayTime();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // アプリがバックグラウンドに入る時にプレイ時間を記録
        if (pauseStatus)
        {
            RecordSessionPlayTime();
        }
        else
        {
            // フォアグラウンドに戻った時は計測再開
            sessionStartTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// このセッションのプレイ時間を記録
    /// </summary>
    private void RecordSessionPlayTime()
    {
        if (!isTracking) return;
        
        float sessionTime = Time.realtimeSinceStartup - sessionStartTime;
        
        if (sessionTime > 0f)
        {
            GameStatistics.AddPlayTime(sessionTime);
            Debug.Log($"[GamePlayTracker] プレイ時間記録: {sessionTime:F2}秒（累計: {GameStatistics.TotalPlayTime:F2}秒）");
        }
        
        // 記録後、計測を再開（次の記録のため）
        sessionStartTime = Time.realtimeSinceStartup;
    }
}
