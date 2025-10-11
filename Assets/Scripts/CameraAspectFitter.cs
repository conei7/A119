using UnityEngine;

/// <summary>
/// カメラの表示範囲を画面比率に合わせて調整し、
/// 常に同じワールド範囲が見えるようにする
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAspectFitter : MonoBehaviour
{
    [Header("基準設定")]
    [Tooltip("基準となるアスペクト比（幅/高さ）。例: 16/9 = 1.777")]
    [SerializeField] private float targetAspect = 16f / 9f;
    
    [Tooltip("基準アスペクト比での縦方向の表示サイズ")]
    [SerializeField] private float targetOrthographicSize = 6f;
    
    [Header("フィット方法")]
    [Tooltip("true: 全体が見える（縦横に余白が出る可能性）\nfalse: 画面いっぱい（一部が見切れる可能性）")]
    [SerializeField] private bool fitInside = true;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void Update()
    {
        // エディタでの変更に対応するため毎フレーム更新
        // ビルド版では Start() だけでも可
        AdjustCamera();
    }

    private void AdjustCamera()
    {
        if (!cam.orthographic) return;

        float currentAspect = (float)Screen.width / Screen.height;
        
        if (fitInside)
        {
            // 全体が見えるように調整（余白が出る）
            if (currentAspect > targetAspect)
            {
                // 画面が横長 → 縦方向を基準にする
                cam.orthographicSize = targetOrthographicSize;
            }
            else
            {
                // 画面が縦長 → 横方向を基準にする
                float targetWidth = targetOrthographicSize * 2f * targetAspect;
                float currentHeight = targetWidth / currentAspect;
                cam.orthographicSize = currentHeight / 2f;
            }
        }
        else
        {
            // 画面いっぱいに表示（一部が見切れる）
            if (currentAspect < targetAspect)
            {
                // 画面が縦長 → 縦方向を基準にする
                cam.orthographicSize = targetOrthographicSize;
            }
            else
            {
                // 画面が横長 → 横方向を基準にする
                float targetWidth = targetOrthographicSize * 2f * targetAspect;
                float currentHeight = targetWidth / currentAspect;
                cam.orthographicSize = currentHeight / 2f;
            }
        }
    }
}
