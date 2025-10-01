using UnityEngine;
using TMPro;

public class LeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Set(int rank, string name, float score)
    {
        if (rankText) rankText.text = rank.ToString();
        if (nameText) nameText.text = name;
        if (scoreText) scoreText.text = score.ToString("F2");
    }
}
