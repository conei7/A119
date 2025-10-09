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
        SwitchLanguage(toggle.isOn);
    }

    void SwitchLanguage(bool isJapanese)
    {
        foreach (var go in englishObjects) go.SetActive(!isJapanese);
        foreach (var go in japaneseObjects) go.SetActive(isJapanese);
    }
}