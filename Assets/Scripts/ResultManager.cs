using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // スコア表示用のTextコンポーネント

    void Start()
    {
        float score = PlayerPrefs.GetFloat("Score", 0f); // スコアを取得（デフォルト値は0）
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("F2"); // スコアを表示
        }
        else
        {
            Debug.LogWarning("ResultManager: scoreText が設定されていません。Inspectorで設定してください。");
        }
    }
}