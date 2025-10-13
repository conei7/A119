using UnityEngine;

/// <summary>
/// 音量設定をデバッグするための一時的なスクリプト
/// </summary>
public class VolumeDebugger : MonoBehaviour
{
    void Start()
    {
        // 保存されている音量を確認
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", -1f);
        float seVol = PlayerPrefs.GetFloat("SEVolume", -1f);
        
        Debug.Log($"保存されているBGM音量: {bgmVol}");
        Debug.Log($"保存されているSE音量: {seVol}");
        
        // もし0になっていたらリセット
        if (bgmVol <= 0.01f || seVol <= 0.01f)
        {
            Debug.LogWarning("音量が0に近い値です。リセットします。");
            PlayerPrefs.SetFloat("BGMVolume", 0.8f);
            PlayerPrefs.SetFloat("SEVolume", 0.8f);
            PlayerPrefs.Save();
            Debug.Log("音量を0.8にリセットしました。");
        }
        
        // SoundManager の状態を確認
        if (SoundManager.Instance != null)
        {
            Debug.Log($"SoundManager BGM音量: {SoundManager.Instance.GetBGMVolume()}");
            Debug.Log($"SoundManager SE音量: {SoundManager.Instance.GetSEVolume()}");
        }
        else
        {
            Debug.LogError("SoundManager.Instance が null です！");
        }
    }
    
    // Spaceキーで音量を強制的に0.8にリセット
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F1キーが押されました。音量をリセットします。");
            
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetBGMVolume(0.8f);
                SoundManager.Instance.SetSEVolume(0.8f);
                Debug.Log("音量を0.8にリセットしました。");
                
                // テストSEを再生
                SoundManager.Instance.PlaySEButtonClick();
            }
        }
    }
}
