using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // スコア表示用
    [SerializeField] private TextMeshProUGUI bestScoreText; // ベストスコア表示用
    [SerializeField] private bool isGameOver = false; // GameOverシーンならtrue

    void Start()
    {
        float score = PlayerPrefs.GetFloat("Score", 0f);
        Debug.Log($"[ResultManager] Score from PlayerPrefs: {score}, IsGameOver: {isGameOver}");
        
        if (scoreText != null)
        {
            if (isGameOver)
            {
                // GameOverの場合：「もしクリアできていたら」の仮想スコア
                scoreText.text = "Final Score: " + score.ToString("F2");
                Debug.Log($"[ResultManager] Set scoreText to: {scoreText.text}");
            }
            else
            {
                // GameClearの場合：実際のクリアスコア
                scoreText.text = "Score: " + score.ToString("F2");
                Debug.Log($"[ResultManager] Set scoreText to: {scoreText.text}");
            }
        }
        else
        {
            Debug.LogWarning("[ResultManager] scoreText is null!");
        }

        // ベストスコア（ランキング1位）を表示
        DisplayBestScore();
    }

    private void DisplayBestScore()
    {
        if (bestScoreText == null) return;

        var topEntries = LeaderboardService.GetTop(1);
        if (topEntries.Count > 0)
        {
            float bestScore = topEntries[0].score;
            string bestName = topEntries[0].name;
            bestScoreText.text = $"Best: {bestScore:F2} ({bestName})";
        }
        else
        {
            bestScoreText.text = "Best: ---";
        }
    }
}