using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 統計画面のUI表示を管理
/// 設定シーンなどに配置して統計情報を表示
/// </summary>
public class StatisticsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button resetButton; // オプション：統計リセットボタン
    [SerializeField] private GameObject confirmResetPanel; // オプション：リセット確認パネル

    [Header("Display Settings")]
    [SerializeField] private bool autoRefresh = true;
    [SerializeField] private float refreshInterval = 1f;

    private float lastRefreshTime;

    void Start()
    {
        // リセットボタンがあれば設定
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClick);
        }

        // 確認パネルがあれば非表示に
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }

        // 初回表示
        RefreshDisplay();
    }

    void Update()
    {
        if (autoRefresh && Time.time - lastRefreshTime >= refreshInterval)
        {
            RefreshDisplay();
        }
    }

    /// <summary>
    /// 統計表示を更新
    /// </summary>
    public void RefreshDisplay()
    {
        if (statsText != null)
        {
            statsText.text = GameStatistics.GetFormattedStats();
            lastRefreshTime = Time.time;
        }
    }

    /// <summary>
    /// リセットボタンクリック時
    /// </summary>
    private void OnResetButtonClick()
    {
        if (confirmResetPanel != null)
        {
            // 確認パネルを表示
            confirmResetPanel.SetActive(true);
        }
        else
        {
            // 確認なしで即リセット
            ResetStatistics();
        }
    }

    /// <summary>
    /// 統計をリセット（確認後）
    /// </summary>
    public void ConfirmReset()
    {
        ResetStatistics();
        
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }

    /// <summary>
    /// リセットをキャンセル
    /// </summary>
    public void CancelReset()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 統計をリセット
    /// </summary>
    private void ResetStatistics()
    {
        GameStatistics.ResetStatistics();
        RefreshDisplay();
        
        // サウンド再生
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEButtonClick();
        }
        
        Debug.Log("[StatisticsUI] 統計をリセットしました");
    }
}
