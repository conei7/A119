using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 開発用：F12キーで全セーブデータを削除するスクリプト
/// リリース時には削除するか、無効化してください
/// </summary>
public class DebugDataReset : MonoBehaviour
{
    void Update()
    {
        // Ctrl+Shift+F12キーが押されたら実行
        if (Input.GetKeyDown(KeyCode.F12) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            // 全てのPlayerPrefsデータを削除
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // ランキングデータも削除（ファイル保存されている場合があるため）
            LeaderboardService.ClearAll();
            
            Debug.Log("【Debug】全セーブデータ・ランキングを削除しました");
            
            // 音を鳴らす（SoundManagerがあれば）
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySEButtonClick();
            }

            // 現在のシーンをリロードして反映
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
