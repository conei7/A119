using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // スコア表示用
    [SerializeField] private TextMeshProUGUI bestScoreText; // ベストスコア表示用
    [SerializeField] private TextMeshProUGUI rankText; // 順位表示用
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
        
        // 順位を表示
        DisplayRank(score);
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

    private void DisplayRank(float score)
    {
        if (rankText == null) return;

        // まだスコアを送信していない場合は、仮の順位を計算
        int rank = LeaderboardService.GetRankForScore(score);
        int totalEntries = LeaderboardService.GetTop(1000).Count; // 全エントリ数を取得

        if (totalEntries == 0)
        {
            // まだ誰も記録がない場合
            rankText.text = "Your rank: 1st (First record!)";
        }
        else
        {
            // 順位を表示（1st, 2nd, 3rd, 4th...）
            string rankSuffix = GetRankSuffix(rank);
            rankText.text = $"Your rank: {rank}{rankSuffix}";
        }

        Debug.Log($"[ResultManager] Rank: {rank}, Total Entries: {totalEntries}");
    }

    /// <summary>
    /// 順位の接尾辞を取得（1st, 2nd, 3rd, 4th...）
    /// </summary>
    private string GetRankSuffix(int rank)
    {
        if (rank % 100 >= 11 && rank % 100 <= 13)
        {
            return "th"; // 11th, 12th, 13th
        }

        switch (rank % 10)
        {
            case 1: return "st"; // 1st, 21st, 31st...
            case 2: return "nd"; // 2nd, 22nd, 32nd...
            case 3: return "rd"; // 3rd, 23rd, 33rd...
            default: return "th"; // 4th, 5th, 6th...
        }
    }
}