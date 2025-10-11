using UnityEngine;
using UnityEngine.UI;

public sealed class LanguageToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject[] englishObjects;
    [SerializeField] private GameObject[] japaneseObjects;

    void Awake()
    {
        toggle.onValueChanged.AddListener(SwitchLanguage);
        // 初期表示は英語(Toggle OFF状態)
        toggle.isOn = false;
        SwitchLanguage(toggle.isOn);
    }

    void SwitchLanguage(bool toggleValue)
    {
        // Toggle OFF(false) = 英語表示、Toggle ON(true) = 日本語表示
        foreach (var go in englishObjects) go.SetActive(!toggleValue);
        foreach (var go in japaneseObjects) go.SetActive(toggleValue);
    }
}