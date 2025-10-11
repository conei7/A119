using UnityEngine;

/// <summary>
/// ワールド座標のオブジェクトをCanvas（World Space）のRectTransformに同期させる
/// 物理演算はワールド座標で行い、表示だけCanvasで行う
/// </summary>
public class WorldToCanvasSync : MonoBehaviour
{
    [Header("同期設定")]
    [Tooltip("同期元のワールド座標オブジェクト")]
    [SerializeField] private Transform worldObject;
    
    [Tooltip("同期先のCanvas（World Space）上のRectTransform")]
    [SerializeField] private RectTransform canvasTarget;
    
    [Tooltip("ワールド座標からCanvasへの変換スケール")]
    [SerializeField] private float scale = 100f; // 1ワールド単位 = 100Canvas単位

    void LateUpdate()
    {
        if (worldObject == null || canvasTarget == null) return;

        // ワールド座標をCanvas座標に変換
        Vector3 worldPos = worldObject.position;
        canvasTarget.anchoredPosition = new Vector2(
            worldPos.x * scale,
            worldPos.y * scale
        );
        
        // 回転も同期
        canvasTarget.rotation = worldObject.rotation;
        
        // スケールも同期（オプション）
        canvasTarget.localScale = worldObject.localScale;
    }
}
