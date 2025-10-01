using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Text scoreText; // スコア表示用のTextコンポーネント

    void Start()
    {
        float score = PlayerPrefs.GetFloat("Score", 0f); // orbitSpeedを取得（デフォルト値は0）
        scoreText.text = "Score: " + score.ToString("F2"); // orbitSpeedを表示
    }
}