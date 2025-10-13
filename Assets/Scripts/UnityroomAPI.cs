using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// unityroom用のランキングAPI連携クラス
/// WebGL専用
/// </summary>
public static class UnityroomAPI
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void unityRoomSendScore(int score);
#endif

    /// <summary>
    /// unityroomにスコアを送信
    /// WebGLビルド時のみ動作
    /// </summary>
    /// <param name="score">送信するスコア（整数）</param>
    public static void SendScore(int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            unityRoomSendScore(score);
            Debug.Log($"[UnityroomAPI] スコアを送信: {score}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[UnityroomAPI] スコア送信失敗: {e.Message}");
        }
#else
        Debug.Log($"[UnityroomAPI] (Editor/非WebGL) スコア送信シミュレート: {score}");
#endif
    }

    /// <summary>
    /// unityroomにスコアを送信（float版）
    /// 小数点以下を切り捨てて整数に変換
    /// </summary>
    public static void SendScore(float score)
    {
        SendScore(Mathf.FloorToInt(score));
    }

    /// <summary>
    /// WebGLビルドかどうかを判定
    /// </summary>
    public static bool IsWebGL()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}
