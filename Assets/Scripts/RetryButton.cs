using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// リトライボタン（タップ/クリック対応）
/// ImageやTextに追加して、タップでゲームをリトライできるようにする
/// </summary>
public class RetryButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [Tooltip("リトライ時に再生するSE")]
    [SerializeField] private bool playSoundOnClick = true;

    /// <summary>
    /// ポインタークリック時（タップやクリック）
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        RetryGame();
    }

    /// <summary>
    /// ゲームをリトライ（現在のシーンをリロード）
    /// </summary>
    public void RetryGame()
    {
        Debug.Log("[RetryButton] リトライ - シーンをリロードします");
        
        // リトライSEを再生
        if (playSoundOnClick && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEButtonClick();
        }
        
        // Time.timeScale を戻す
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
        
        // 現在のシーンをリロード
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
