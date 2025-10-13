using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public float score;
    public long timestamp; // Unix time (seconds)
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public static class LeaderboardService
{
    private static string FilePath
    {
        get
        {
            return System.IO.Path.Combine(Application.persistentDataPath, FILENAME);
        }
    }
    
    // WebGL用のLocalStorageキー
    private const string WEBGL_STORAGE_KEY = "LeaderboardData";
    private const int MaxStored = 1000; // 保存上限（必要に応じて調整）
    // 日本語や全角文字も許可
    private static readonly Regex AllowedName = new Regex(@"[^\p{L}\p{N} _-]", RegexOptions.Compiled);
    private const int MaxNameLength = 16;

    public static string SanitizeName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "Player";
        string trimmed = raw.Trim();
        // 許可以外の文字を除去
        string onlyAscii = AllowedName.Replace(trimmed, "");
        if (string.IsNullOrEmpty(onlyAscii)) onlyAscii = "Player";
        if (onlyAscii.Length > MaxNameLength) onlyAscii = onlyAscii.Substring(0, MaxNameLength);
        return onlyAscii;
    }

    public static void AddScore(string name, float score)
    {
        var data = Load();
        Debug.Log($"[LeaderboardService] 現在のエントリ数: {data.entries.Count}");
        
        var entry = new LeaderboardEntry
        {
            name = SanitizeName(name),
            score = score,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        data.entries.Add(entry);
        Debug.Log($"[LeaderboardService] エントリ追加: {entry.name} - {entry.score:F2}");
        
        SortDescending(data.entries);
        if (data.entries.Count > MaxStored)
            data.entries.RemoveRange(MaxStored, data.entries.Count - MaxStored);
        
        Save(data);
        Debug.Log($"[LeaderboardService] 保存完了。ファイルパス: {FilePath}");
        Debug.Log($"[LeaderboardService] 保存後のエントリ数: {data.entries.Count}");
    }

    public static List<LeaderboardEntry> GetTop(int count)
    {
        var data = Load();
        Debug.Log($"[LeaderboardService] GetTop 呼び出し。読み込んだエントリ数: {data.entries.Count}");
        
        SortDescending(data.entries);
        if (count < 0) count = 0;
        if (count > data.entries.Count) count = data.entries.Count;
        
        var result = data.entries.GetRange(0, count);
        Debug.Log($"[LeaderboardService] 返すエントリ数: {result.Count}");
        
        return result;
    }

    public static int GetRankForScore(float score)
    {
        var data = Load();
        SortDescending(data.entries);
        for (int i = 0; i < data.entries.Count; i++)
        {
            if (score >= data.entries[i].score) return i + 1; // 1-based
        }
        return data.entries.Count + 1;
    }

    public static void ClearAll()
    {
        var data = new LeaderboardData();
        Save(data);
    }

    private static LeaderboardData Load()
    {
        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // WebGLではPlayerPrefsを使用（F5でもデータ保持）
            string json = PlayerPrefs.GetString(WEBGL_STORAGE_KEY, "");
            if (string.IsNullOrWhiteSpace(json)) return new LeaderboardData();
            return JsonUtility.FromJson<LeaderboardData>(json) ?? new LeaderboardData();
#else
            if (!File.Exists(FilePath)) return new LeaderboardData();
            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return new LeaderboardData();
            return JsonUtility.FromJson<LeaderboardData>(json) ?? new LeaderboardData();
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard load failed: {e.Message}");
            return new LeaderboardData();
        }
    }

    private static void Save(LeaderboardData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
#if UNITY_WEBGL && !UNITY_EDITOR
            // WebGLではPlayerPrefsに保存（LocalStorageに永続化）
            PlayerPrefs.SetString(WEBGL_STORAGE_KEY, json);
            PlayerPrefs.Save();
#else
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
            File.WriteAllText(FilePath, json);
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard save failed: {e.Message}");
        }
    }

    private static void SortDescending(List<LeaderboardEntry> list)
    {
        list.Sort((a, b) =>
        {
            int byScore = b.score.CompareTo(a.score); // 高スコア先
            if (byScore != 0) return byScore;
            return a.timestamp.CompareTo(b.timestamp); // 早い登録が先
        });
    }
}
