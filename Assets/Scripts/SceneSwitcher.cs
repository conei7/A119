using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string nextSceneName; // 遷移先のシーン名

    // シーン遷移を実行するメソッド
    public void SwitchScene()
    {
        SceneManager.LoadScene(nextSceneName); // シーン遷移
    }
}