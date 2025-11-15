using System;
using UnityEngine;

/// <summary>
/// ゲーム全体の統計情報を管理する静的クラス。
/// PlayerPrefs を通じて WebGL でも永続化される。
/// </summary>
public static class GameStatistics
{
    // ========= PlayerPrefs Keys =========
    private const string KEY_TOTAL_PLAY_TIME = "Stats_TotalPlayTime";
    private const string KEY_START_COUNT = "Stats_StartCount";
    private const string KEY_BEST_SCORE = "Stats_BestScore";
    private const string KEY_MAX_SPEED = "Stats_MaxSpeed";
    private const string KEY_TOTAL_CLEARS = "Stats_TotalClears";
    private const string KEY_TOTAL_GAME_OVERS = "Stats_TotalGameOvers";
    private const string KEY_FIRST_PLAY_DATE = "Stats_FirstPlayDate";
    private const string KEY_LAST_PLAY_DATE = "Stats_LastPlayDate";

    // ========= Public Properties =========
    public static float TotalPlayTime
    {
        get => PlayerPrefs.GetFloat(KEY_TOTAL_PLAY_TIME, 0f);
        private set => PlayerPrefs.SetFloat(KEY_TOTAL_PLAY_TIME, value);
    }

    public static int StartCount
    {
        get => PlayerPrefs.GetInt(KEY_START_COUNT, 0);
        private set => PlayerPrefs.SetInt(KEY_START_COUNT, value);
    }

    public static float BestScore
    {
        get => PlayerPrefs.GetFloat(KEY_BEST_SCORE, 0f);
        private set => PlayerPrefs.SetFloat(KEY_BEST_SCORE, value);
    }

    public static float MaxSpeed
    {
        get => PlayerPrefs.GetFloat(KEY_MAX_SPEED, 0f);
        private set => PlayerPrefs.SetFloat(KEY_MAX_SPEED, value);
    }

    public static int TotalClears
    {
        get => PlayerPrefs.GetInt(KEY_TOTAL_CLEARS, 0);
        private set => PlayerPrefs.SetInt(KEY_TOTAL_CLEARS, value);
    }

    public static int TotalGameOvers
    {
        get => PlayerPrefs.GetInt(KEY_TOTAL_GAME_OVERS, 0);
        private set => PlayerPrefs.SetInt(KEY_TOTAL_GAME_OVERS, value);
    }

    public static string FirstPlayDate
    {
        get => PlayerPrefs.GetString(KEY_FIRST_PLAY_DATE, string.Empty);
        private set => PlayerPrefs.SetString(KEY_FIRST_PLAY_DATE, value);
    }

    public static string LastPlayDate
    {
        get => PlayerPrefs.GetString(KEY_LAST_PLAY_DATE, string.Empty);
        private set => PlayerPrefs.SetString(KEY_LAST_PLAY_DATE, value);
    }

    // ========= Recording Methods =========

    /// <summary>
    /// ゲーム開始時に呼び出し、プレイ回数や日時を記録。
    /// </summary>
    public static void RecordGameStart()
    {
        StartCount = StartCount + 1;

        DateTime jstNow = GetJapanTime();
        string formatted = jstNow.ToString("yyyy/MM/dd HH:mm");

        if (string.IsNullOrEmpty(FirstPlayDate))
        {
            FirstPlayDate = formatted;
        }

        LastPlayDate = formatted;
        PlayerPrefs.Save();

        Debug.Log($"[GameStatistics] ゲームスタート記録: {StartCount}回目");
    }

    /// <summary>
    /// セッション中に経過したプレイ時間を加算。
    /// </summary>
    public static void AddPlayTime(float seconds)
    {
        if (seconds <= 0f) return;

        TotalPlayTime = TotalPlayTime + seconds;
        PlayerPrefs.Save();
    }

    /// <summary>
    /// クリア実績を記録し、最高スコア/速度を更新。
    /// </summary>
    public static void RecordClear(float score)
    {
        TotalClears = TotalClears + 1;

        if (score > BestScore)
        {
            BestScore = score;
            Debug.Log($"[GameStatistics] 新記録! 最高スコア: {score:F2}");
        }

        if (score > MaxSpeed)
        {
            MaxSpeed = score;
        }

        PlayerPrefs.Save();

        Debug.Log($"[GameStatistics] クリア記録: {TotalClears}回目, スコア: {score:F2}");
    }

    /// <summary>
    /// ゲームオーバー時の最高速度を記録。
    /// </summary>
    public static void RecordGameOver(float maxSpeedReached)
    {
        TotalGameOvers = TotalGameOvers + 1;

        if (maxSpeedReached > MaxSpeed)
        {
            MaxSpeed = maxSpeedReached;
            Debug.Log($"[GameStatistics] 新記録! 最高速度: {maxSpeedReached:F2}");
        }

        PlayerPrefs.Save();

        Debug.Log($"[GameStatistics] ゲームオーバー記録: {TotalGameOvers}回目, 速度: {maxSpeedReached:F2}");
    }

    /// <summary>
    /// PlayerPrefs に保存された統計をすべて削除。
    /// </summary>
    public static void ResetStatistics()
    {
        PlayerPrefs.DeleteKey(KEY_TOTAL_PLAY_TIME);
        PlayerPrefs.DeleteKey(KEY_START_COUNT);
        PlayerPrefs.DeleteKey(KEY_BEST_SCORE);
        PlayerPrefs.DeleteKey(KEY_MAX_SPEED);
        PlayerPrefs.DeleteKey(KEY_TOTAL_CLEARS);
        PlayerPrefs.DeleteKey(KEY_TOTAL_GAME_OVERS);
        PlayerPrefs.DeleteKey(KEY_FIRST_PLAY_DATE);
        PlayerPrefs.DeleteKey(KEY_LAST_PLAY_DATE);
        PlayerPrefs.Save();

        Debug.Log("[GameStatistics] 統計データをリセットしました");
    }

    // ========= Rendering / Debug =========

    public static string GetFormattedStats()
    {
        string stats = string.Empty;

        stats += $"Total Starts: {StartCount}\n";
        stats += $"Total Clears: {TotalClears}\n";
        stats += $"Total Game Overs: {TotalGameOvers}\n";

        int retryCount = StartCount - TotalClears - TotalGameOvers;
        if (retryCount > 0)
        {
            stats += $"Retries: {retryCount}\n";
        }
        stats += "\n";

        stats += "Best Score: ";
        stats += BestScore > 0f ? $"{BestScore:F2}\n" : "---\n";

        stats += $"Max Speed Ever: {MaxSpeed:F2}\n";
        stats += $"Total Play Time: {FormatTime(TotalPlayTime)}\n\n";

        if (!string.IsNullOrEmpty(FirstPlayDate))
        {
            stats += $"First Play: {FirstPlayDate}\n";
        }
        if (!string.IsNullOrEmpty(LastPlayDate))
        {
            stats += $"Last Play: {LastPlayDate}\n";
        }

        return stats;
    }

    public static void PrintStats()
    {
        Debug.Log(GetFormattedStats());
    }

    // ========= Helpers =========

    private static string FormatTime(float totalSeconds)
    {
        if (totalSeconds < 60f)
        {
            return $"{totalSeconds:F1}s";
        }

        if (totalSeconds < 3600f)
        {
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            float seconds = totalSeconds % 60f;
            return $"{minutes}m {seconds:F0}s";
        }

        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int mins = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int secs = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{hours}h {mins}m {secs}s";
    }

    private static DateTime GetJapanTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan jstOffset = TimeSpan.FromHours(9);
        return utcNow.Add(jstOffset);
    }
}
