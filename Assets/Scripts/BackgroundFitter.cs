using UnityEngine;

// 2D背景スプライトをカメラサイズに合わせて画面いっぱいにフィットさせる
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitter : MonoBehaviour
{
    [Tooltip("背景を追従させる対象カメラ。未指定ならMain Camera")] 
    public Camera targetCamera;
    
    [Tooltip("true: 余白が出ないように全面をカバー(一部トリミングあり)。false: 全体が収まる(左右/上下に余白が出る)")] 
    public bool cover = true;
    
    [Tooltip("背景の中心を常にカメラ中心に合わせる")] 
    public bool alignToCameraCenter = true;

    private SpriteRenderer sr;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        // カメラ中心に合わせる
        if (alignToCameraCenter)
        {
            transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
        }

        if (cam.orthographic)
        {
            float worldHeight = cam.orthographicSize * 2f;
            float worldWidth  = worldHeight * cam.aspect;
            FitToSize(worldWidth, worldHeight);
        }
        else
        {
            // 透視カメラの場合
            float distance = Mathf.Abs(transform.position.z - cam.transform.position.z);
            float frustumHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth  = frustumHeight * cam.aspect;
            FitToSize(frustumWidth, frustumHeight);
        }
    }

    private void FitToSize(float targetW, float targetH)
    {
        Vector2 spriteSize = sr.sprite.bounds.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f) return;

        float sx = targetW / spriteSize.x;
        float sy = targetH / spriteSize.y;
        float s = cover ? Mathf.Max(sx, sy) : Mathf.Min(sx, sy);

        transform.localScale = new Vector3(s, s, 1f);
    }
}
