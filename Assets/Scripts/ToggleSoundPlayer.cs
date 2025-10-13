using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ToggleのonValueChangedイベントから音を再生するヘルパークラス
/// Toggleと同じGameObjectにアタッチして使用
/// </summary>
public class ToggleSoundPlayer : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }
    }

    private void Start()
    {
        if (toggle != null)
        {
            // 値設定後にリスナーを登録（初期化時に音が鳴らないようにする）
            toggle.onValueChanged.AddListener(PlayToggleSound);
        }
    }

    /// <summary>
    /// Toggle切り替え時にSEを再生
    /// </summary>
    private void PlayToggleSound(bool isOn)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEButtonClick();
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(PlayToggleSound);
        }
    }
}
