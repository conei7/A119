using UnityEngine;

/// <summary>
/// ゲームオーバー画面でオブジェクトを左から右へ定速で流す演出用スクリプト。
/// プレイヤーの最終スコアを速度として利用し、Among Us の追放演出のような動きを実現します。
/// </summary>
public class GameOverEjector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform movingTarget; // 移動・回転させる対象。未指定なら自身
    [SerializeField] private Transform startPoint;   // スタート地点（画面左側など）
    [SerializeField] private Transform endPoint;     // 終了地点（画面右側など）

    [Header("Motion Settings")]
    [SerializeField, Tooltip("スコア速度に掛ける倍率。1でスコアそのまま")] private float speedMultiplier = 1f;
    [SerializeField, Min(0f), Tooltip("移動速度の下限値。スコアが小さすぎてもこれを下回らない")] private float minimumSpeed = 0f;
    [SerializeField, Tooltip("Z回転の角速度(度/秒)")] private float rotationSpeed = 120f;
    [SerializeField, Tooltip("ループ開始時にスタート地点へスナップする")] private bool snapToStartOnEnable = true;
    [SerializeField, Tooltip("Endに到達したあと再スタートするまでの待機時間(秒)")] private float endPauseDuration = 0f;

    [Header("Score Settings")]
    [SerializeField, Tooltip("速度として利用するPlayerPrefsキー")] private string scorePrefsKey = "Score";
    [SerializeField, Tooltip("スコアの絶対値を用いるか(負値対策)")] private bool useAbsoluteScore = true;
    [SerializeField, Tooltip("スコアを定期的に読み直して速度を更新する")]
    private bool autoRefreshSpeed = false;
    [SerializeField, Tooltip("autoRefreshSpeed が有効な時の更新間隔(秒)")] private float refreshInterval = 0.5f;
    [SerializeField, Tooltip("スコアと速度をDebug.Logに出力する")] private bool logSpeed = false;

    private float moveSpeed;
    private float pathLength;
    private Vector3 pathDirection = Vector3.right;
    private float travelled;
    private float nextRefreshTime;
    private bool pausedAtEnd;
    private float resumeTime;

    private void Awake()
    {
        if (movingTarget == null)
        {
            movingTarget = transform;
        }
    }

    private void OnEnable()
    {
        InitialisePath();
        FetchSpeedFromScore();
        ResetPosition();
        if (autoRefreshSpeed)
        {
            nextRefreshTime = Time.time + Mathf.Max(0.01f, refreshInterval);
        }
    }

    private void InitialisePath()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning("[GameOverEjector] startPoint / endPoint が設定されていません。移動は行われません。");
            pathLength = 0f;
            pathDirection = Vector3.right;
            return;
        }

        Vector3 diff = endPoint.position - startPoint.position;
        pathLength = diff.magnitude;
        if (pathLength < 0.001f)
        {
            pathLength = 1f;
            pathDirection = Vector3.right;
        }
        else
        {
            pathDirection = diff / pathLength;
        }
    }

    private void FetchSpeedFromScore()
    {
        float score = PlayerPrefs.GetFloat(scorePrefsKey, minimumSpeed);
        if (useAbsoluteScore)
        {
            score = Mathf.Abs(score);
        }

        float computedSpeed = score * speedMultiplier;
        if (computedSpeed < minimumSpeed)
        {
            computedSpeed = minimumSpeed;
        }

        if (logSpeed)
        {
            Debug.Log($"[GameOverEjector] score={score:F3}, speed={computedSpeed:F3}");
        }

        moveSpeed = computedSpeed;
    }

    private void ResetPosition()
    {
        travelled = 0f;
        if (snapToStartOnEnable && startPoint != null && movingTarget != null)
        {
            movingTarget.position = startPoint.position;
        }
        pausedAtEnd = false;
        resumeTime = 0f;
    }

    private void Update()
    {
        if (movingTarget == null || pathLength <= 0f || moveSpeed <= 0f)
        {
            return;
        }

        if (autoRefreshSpeed && Time.time >= nextRefreshTime)
        {
            FetchSpeedFromScore();
            nextRefreshTime = Time.time + Mathf.Max(0.01f, refreshInterval);
        }

        if (pausedAtEnd)
        {
            if (Time.time >= resumeTime)
            {
                pausedAtEnd = false;
                // End地点までの端数を詰めてリスタートする
                travelled = Mathf.Repeat(travelled, pathLength);
            }
            else
            {
                // 停止中は位置と回転を固定しておく
                return;
            }
        }

        float nextTravelled = travelled + moveSpeed * Time.deltaTime;
        if (pathLength > 0f && nextTravelled >= pathLength)
        {
            if (endPauseDuration > 0f)
            {
                travelled = pathLength;
                movingTarget.position = startPoint.position + pathDirection * pathLength;
                pausedAtEnd = true;
                resumeTime = Time.time + endPauseDuration;
                return;
            }
        }

        travelled = Mathf.Repeat(nextTravelled, pathLength);
        movingTarget.position = startPoint.position + pathDirection * travelled;

        if (rotationSpeed != 0f)
        {
            movingTarget.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawSphere(startPoint.position, 0.15f);
            Gizmos.DrawSphere(endPoint.position, 0.15f);
        }
    }
}
