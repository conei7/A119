using UnityEngine;
using UnityEngine.UI;

public sealed class LanguageToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject[] englishObjects;
    [SerializeField] private GameObject[] japaneseObjects;

    void Awake()
    {
        // 初期表示は英語(Toggle OFF状態)
        // リスナー登録前に値を設定することで、イベントを発火させない
        toggle.isOn = false;
        SwitchLanguage(toggle.isOn);
        
        // 値設定後にリスナーを登録
        toggle.onValueChanged.AddListener(SwitchLanguage);
    }

    void SwitchLanguage(bool toggleValue)
    {
        // Toggle OFF(false) = 英語表示、Toggle ON(true) = 日本語表示
        foreach (var go in englishObjects) go.SetActive(!toggleValue);
        foreach (var go in japaneseObjects) go.SetActive(toggleValue);
    }
}