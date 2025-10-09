using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager instance not found! Make sure SoundManager exists in the scene.");
            return;
        }

        InitializeSliders();
    }

    /// <summary>
    /// スライダーの初期値を設定し、イベントリスナーを登録
    /// </summary>
    private void InitializeSliders()
    {
        // BGMスライダーの初期化
        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            // 値を設定する前にリスナーを削除(念のため)
            bgmSlider.onValueChanged.RemoveAllListeners();
            // 値を設定
            bgmSlider.value = SoundManager.Instance.GetBGMVolume();
            // 値設定後にリスナーを登録
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        else
        {
            Debug.LogWarning("BGM Slider is not assigned!");
        }

        // SEスライダーの初期化
        if (seSlider != null)
        {
            seSlider.minValue = 0f;
            seSlider.maxValue = 1f;
            // 値を設定する前にリスナーを削除(念のため)
            seSlider.onValueChanged.RemoveAllListeners();
            // 値を設定
            seSlider.value = SoundManager.Instance.GetSEVolume();
            // 値設定後にリスナーを登録
            seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }
        else
        {
            Debug.LogWarning("SE Slider is not assigned!");
        }
    }

    /// <summary>
    /// BGMスライダーの値が変更されたときに呼ばれる
    /// </summary>
    private void OnBGMVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(value);
        }
    }

    /// <summary>
    /// SEスライダーの値が変更されたときに呼ばれる
    /// </summary>
    private void OnSEVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSEVolume(value);
        }
    }

    private void OnDestroy()
    {
        // イベントリスナーを解除
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        }

        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveListener(OnSEVolumeChanged);
        }
    }
}
