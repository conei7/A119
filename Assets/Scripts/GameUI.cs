using UnityEngine;
using TMPro;

/// <summary>
/// ゲームシーン用UI（スコア表示など）
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PlayerController player;

    [Header("Display Settings")]
    [SerializeField] private string scorePrefix = "SPEED: ";
    [SerializeField] private string scoreFormat = "F1"; // 小数点1桁

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (scoreText == null)
        {
            Debug.LogWarning("[GameUI] Score Text が設定されていません");
        }
    }

    void Update()
    {
        if (scoreText != null && player != null)
        {
            // PlayerControllerからorbitSpeedを取得して表示
            float currentSpeed = GetCurrentSpeed();
            scoreText.text = scorePrefix + currentSpeed.ToString(scoreFormat);
        }
    }

    /// <summary>
    /// 現在の速度を取得
    /// </summary>
    private float GetCurrentSpeed()
    {
        if (player != null)
        {
            return player.GetCurrentSpeed();
        }
        
        return 0f;
    }
}
