using UnityEngine;

public class GameQuitHandler : MonoBehaviour
{
    void Update()
    {
        // Escキーが押されたらゲームを終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        // Unityエディタで実行中の場合は再生を停止
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("ゲーム終了 (エディタモード)");
#else
        // ビルド版の場合はアプリケーションを終了
        Application.Quit();
        Debug.Log("ゲーム終了");
#endif
    }
}
