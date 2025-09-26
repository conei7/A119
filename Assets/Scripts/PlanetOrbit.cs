using UnityEngine;

/// 惑星(任意オブジェクト)を原点(0,0) もしくは指定中心を基準に公転させる 2D 用スクリプト
public class PlanetOrbit : MonoBehaviour
{
    [Header("軌道パラメータ")]
    [Tooltip("中心。未指定なら (0,0,0) を使用")]
    public Transform center;
    [Tooltip("円/楕円のX半径 (円なら半径)")]
    public float radiusX = 5f;
    [Tooltip("円/楕円のY半径 (円にしたい場合は X と同じ)")]
    public float radiusY = 5f;
    [Tooltip("角速度(度/秒)")]
    public float angularSpeedDeg = 20f;
    [Tooltip("開始角度(度)")]
    public float startAngleDeg = 0f;
    [Tooltip("時計回りにするならオン")]
    public bool clockwise = false;
    [Tooltip("開始時に現在位置から半径と角度を自動取得して上書き")]
    public bool initializeFromCurrentPosition = false;

    private float angleRad;
    private Vector3 centerPos;

    void Start()
    {
        angleRad = startAngleDeg * Mathf.Deg2Rad;
        centerPos = center ? center.position : Vector3.zero;

        if (initializeFromCurrentPosition)
        {
            Vector3 diff = transform.position - centerPos;
            radiusX = Mathf.Abs(diff.x);
            radiusY = Mathf.Abs(diff.y);
            if (radiusX < 0.001f) radiusX = radiusY;
            if (radiusY < 0.001f) radiusY = radiusX;
            if (radiusX < 0.001f) radiusX = radiusY = 1f;

            // 楕円角度推定（スケーリングしてから Atan2）
            float sx = radiusX == 0 ? 1f : radiusX;
            float sy = radiusY == 0 ? 1f : radiusY;
            angleRad = Mathf.Atan2(diff.y / sy, diff.x / sx);
        }
    }

    void Update()
    {
        float dir = clockwise ? -1f : 1f;
        angleRad += dir * angularSpeedDeg * Mathf.Deg2Rad * Time.deltaTime;
        if (angleRad > Mathf.PI * 2f) angleRad -= Mathf.PI * 2f;
        else if (angleRad < 0f) angleRad += Mathf.PI * 2f;

        centerPos = center ? center.position : Vector3.zero;

        float x = centerPos.x + Mathf.Cos(angleRad) * radiusX;
        float y = centerPos.y + Mathf.Sin(angleRad) * radiusY;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 c = center ? center.position : Vector3.zero;
        Gizmos.color = Color.cyan;
        const int SEG = 72;
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= SEG; i++)
        {
            float t = (float)i / SEG * Mathf.PI * 2f;
            float px = c.x + Mathf.Cos(t) * radiusX;
            float py = c.y + Mathf.Sin(t) * radiusY;
            Vector3 p = new Vector3(px, py, 0f);
            if (i > 0) Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }
}