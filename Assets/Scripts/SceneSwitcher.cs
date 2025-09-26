using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 遷移先のシーン名をInspectorで指定
    public string nextSceneName;

    // シーン遷移を実行するメソッド
    public void SwitchScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}