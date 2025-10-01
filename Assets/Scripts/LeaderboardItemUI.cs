using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text scoreText;

    public void Set(int rank, string name, float score)
    {
        if (rankText) rankText.text = rank.ToString();
        if (nameText) nameText.text = name;
        if (scoreText) scoreText.text = score.ToString("F2");
    }
}
