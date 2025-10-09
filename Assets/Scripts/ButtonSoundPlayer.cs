using UnityEngine;

/// <summary>
/// ボタンのOnClick()イベントから呼び出すためのヘルパークラス
/// ボタンと同じGameObjectにアタッチして、OnClick()で PlayButtonClickSE() を呼び出す
/// </summary>
public class ButtonSoundPlayer : MonoBehaviour
{
    /// <summary>
    /// ボタンクリックSEを再生
    /// Button の OnClick() イベントから呼び出す
    /// </summary>
    public void PlayButtonClickSE()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEButtonClick();
        }
        else
        {
            Debug.LogWarning("SoundManager instance not found!");
        }
    }
}
