using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform content; // ScrollView の Content
    [SerializeField] private LeaderboardItemUI itemPrefab; // 行アイテムのプレハブ
    [SerializeField] private int displayCount = 30;

    [Header("Submit (optional)")]
    [SerializeField] private InputField nameInput;
    [SerializeField] private Button submitButton;

    [Header("Score Source")]
    [SerializeField] private bool readLastScoreFromPrefs = true; // PlayerPrefs("Score")
    [SerializeField] private string scorePrefsKey = "Score";

    private float lastScore;

    void Awake()
    {
        if (readLastScoreFromPrefs)
            lastScore = PlayerPrefs.GetFloat(scorePrefsKey, 0f);

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmit);
        }
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // 既存をクリア
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        List<LeaderboardEntry> list = LeaderboardService.GetTop(displayCount);
        for (int i = 0; i < list.Count; i++)
        {
            var ui = Instantiate(itemPrefab, content);
            ui.Set(i + 1, list[i].name, list[i].score);
        }
    }

    public void OnSubmit()
    {
        if (nameInput == null) return;
        string raw = nameInput.text;
        string sanitized = LeaderboardService.SanitizeName(raw);
        LeaderboardService.AddScore(sanitized, lastScore);
        Refresh();
    }
}
