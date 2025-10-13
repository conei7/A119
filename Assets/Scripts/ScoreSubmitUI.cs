using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// GameClearシーンなどでスコアを入力して送信するUI
/// </summary>
public class ScoreSubmitUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI scoreDisplayText; // スコア表示用（オプション）

    [Header("Score Settings")]
    [SerializeField] private string scorePrefsKey = "Score";
    [SerializeField] private string leaderboardSceneName = "Leaderboard";

    private float currentScore;

    void Start()
    {
        // PlayerPrefsから最新スコアを読み込み
        currentScore = PlayerPrefs.GetFloat(scorePrefsKey, 0f);

        // スコア表示（オプション）
        if (scoreDisplayText != null)
        {
            scoreDisplayText.text = $"Score: {currentScore:F2}";
        }

        // InputFieldのモバイル設定
        if (nameInput != null)
        {
            // タッチキーボードを有効化
            nameInput.keyboardType = TouchScreenKeyboardType.Default;
            // 自動選択を有効化（タップしたら即座に入力開始）
            nameInput.Select();
            nameInput.ActivateInputField();
        }

        // ボタンにリスナー登録
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitScore);
        }

        // Enterキーでも送信できるようにする
        if (nameInput != null)
        {
            nameInput.onSubmit.AddListener((text) => OnSubmitScore());
        }
    }

    public void OnSubmitScore()
    {
        if (nameInput == null)
        {
            Debug.LogWarning("Name Input が設定されていません");
            return;
        }

        string rawName = nameInput.text;
        
        // 名前のサニタイズ
        string sanitizedName = LeaderboardService.SanitizeName(rawName);
        
        // スコアを保存
        LeaderboardService.AddScore(sanitizedName, currentScore);
        
        // unityroomにスコアを送信（公式ライブラリ使用）
#if UNITY_WEBGL && !UNITY_EDITOR
        if (unityroom.Api.UnityroomApiClient.Instance != null)
        {
            unityroom.Api.UnityroomApiClient.Instance.SendScore(1, currentScore, unityroom.Api.ScoreboardWriteMode.HighScoreDesc);
        }
#endif
        
        Debug.Log($"スコア送信: {sanitizedName} - {currentScore:F2}");

        // リーダーボードシーンへ遷移（存在チェック付き）
        if (!string.IsNullOrEmpty(leaderboardSceneName))
        {
            if (!Application.CanStreamedLevelBeLoaded(leaderboardSceneName))
            {
                Debug.LogError($"シーン\"{leaderboardSceneName}\"は Build Settings に含まれていません。File > Build Settings... で追加してください。");
                return;
            }
            
            SceneManager.LoadScene(leaderboardSceneName);
        }
    }

    /// <summary>
    /// スコア送信せずにリーダーボードを見る
    /// </summary>
    public void OnViewLeaderboard()
    {
        if (!string.IsNullOrEmpty(leaderboardSceneName))
        {
            if (!Application.CanStreamedLevelBeLoaded(leaderboardSceneName))
            {
                Debug.LogError($"シーン\"{leaderboardSceneName}\"は Build Settings に含まれていません。File > Build Settings... で追加してください。");
                return;
            }
            
            SceneManager.LoadScene(leaderboardSceneName);
        }
    }
}
