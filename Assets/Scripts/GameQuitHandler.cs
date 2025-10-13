using UnityEngine;

public class GameQuitHandler : MonoBehaviour
{
    void Update()
    {
        // Escキーが押されたらゲームを終了（WebGLでは無効）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            QuitGame();
#endif
        }

        // F11キーで全画面切り替え
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreen();
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

    void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            // 全画面から通常ウィンドウに切り替え
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
            Debug.Log("ウィンドウモード: 1920x1080");
        }
        else
        {
            // ウィンドウから全画面に切り替え
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
            Debug.Log("全画面モード");
        }
    }
}
