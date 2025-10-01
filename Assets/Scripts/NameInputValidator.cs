using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class NameInputValidator : MonoBehaviour
{
    [Tooltip("空なら Player に置換")]
    public bool replaceEmptyWithDefault = true;
    public string defaultName = "Player";
    [Tooltip("最大文字数")]
    public int maxLength = 16;

    private InputField input;
    private static readonly Regex allowed = new Regex("[^A-Za-z0-9 _-]", RegexOptions.Compiled);

    void Awake()
    {
        input = GetComponent<InputField>();
        if (input != null)
            input.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string _)
    {
        if (input == null) return;
        string text = input.text;
        text = allowed.Replace(text, "");
        if (text.Length > maxLength) text = text.Substring(0, maxLength);
        if (replaceEmptyWithDefault && string.IsNullOrWhiteSpace(text)) text = defaultName;
        input.text = text;
    }
}
