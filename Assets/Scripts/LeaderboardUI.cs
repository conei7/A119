using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform content; // ScrollView の Content
    [SerializeField] private LeaderboardItemUI itemPrefab; // 行アイテムのプレハブ
    [SerializeField] private int displayCount = 30;

    void Awake()
    {
        // 初期化処理（必要に応じて追加）
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        Debug.Log($"[LeaderboardUI] Refresh 開始");
        
        // 既存をクリア
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if (content == null)
        {
            Debug.LogError("[LeaderboardUI] Content が null です！Inspector で設定してください。");
            return;
        }
        
        if (itemPrefab == null)
        {
            Debug.LogError("[LeaderboardUI] Item Prefab が null です！Inspector で設定してください。");
            return;
        }

        List<LeaderboardEntry> list = LeaderboardService.GetTop(displayCount);
        Debug.Log($"[LeaderboardUI] 取得したエントリ数: {list.Count}");
        
        for (int i = 0; i < list.Count; i++)
        {
            var ui = Instantiate(itemPrefab, content);
            ui.Set(i + 1, list[i].name, list[i].score);
            Debug.Log($"[LeaderboardUI] 行作成: {i + 1}位 - {list[i].name} - {list[i].score:F2}");
        }
        
        Debug.Log($"[LeaderboardUI] Refresh 完了。{list.Count}件表示");
    }


}
