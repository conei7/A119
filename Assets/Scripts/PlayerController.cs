using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private enum PlayerState { OnMoon, Flying, OrbitingPlanet }
    private PlayerState currentState;

    [Header("月からの射出速度")]
    [SerializeField] private float initialLaunchSpeed = 10f;

    private Rigidbody2D rb;
    private Transform orbitingPlanet;
    private float orbitSpeed = 1f;

    private float orbitRadius;
    private float orbitAngle;
    private float orbitAngularSpeed;
    private int orbitDirection = 1;
    private Transform ignoredPlanet;
    private float lastOrbitSpeed = 0f;
    [SerializeField] private float samePlanetReenterDelay = 1.0f;
    private float ignoredPlanetUntil = 0f;

    [Header("シーン遷移設定")]
    [SerializeField, Tooltip("クリア時に遷移するシーン名")] private string gameClearSceneName = "GameClear";
    [SerializeField, Tooltip("ゲームオーバー時に遷移するシーン名")] private string gameOverSceneName = "GameOver";
    private bool hasLaunchedFromMoon = false;
    private bool gameEnded = false;

    [Header("画面外ゲームオーバー設定")]
    [SerializeField, Tooltip("Viewport基準での余白(負側/正側とも)")] private float outMargin = 0.05f;
    [SerializeField] private bool enableOutOfScreenGameOver = true;
    [SerializeField] private string outMessage = "ゲームオーバー: 画面外";
    [SerializeField, Tooltip("画面中心からの許容倍率。1=ちょうど画面、2=画面の2倍領域まではセーフ")] private float screenExtentFactor = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.OnMoon;
        rb.isKinematic = true;
    }

    void Update()
    {
        // Rキーでリトライ
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryGame();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == PlayerState.OnMoon)
            {
                LaunchFromMoon();
            }
            else if (currentState == PlayerState.OrbitingPlanet)
            {
                LaunchFromPlanet();
            }
        }

    // 画面外判定
    CheckOutOfScreen();
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.OrbitingPlanet)
        {
            if (orbitingPlanet == null)
            {
                lastOrbitSpeed = Mathf.Max(lastOrbitSpeed, orbitSpeed);
                currentState = PlayerState.Flying;
                rb.isKinematic = false;
                return;
            }

            orbitAngle += orbitAngularSpeed * orbitDirection * Time.fixedDeltaTime;
            if (orbitAngle > Mathf.PI * 2f) orbitAngle -= Mathf.PI * 2f;
            else if (orbitAngle < 0f) orbitAngle += Mathf.PI * 2f;

            Vector2 center = orbitingPlanet.position;
            Vector2 offset = new Vector2(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitRadius;

            if (!rb.isKinematic) rb.isKinematic = true;
            rb.MovePosition(center + offset);

            Vector2 tangent = new Vector2(-Mathf.Sin(orbitAngle), Mathf.Cos(orbitAngle)) * orbitDirection;
            rb.velocity = tangent * orbitSpeed;
        }
    }

    void LaunchFromMoon()
    {
        currentState = PlayerState.Flying;
        rb.isKinematic = false;
        transform.parent = null;

        Vector2 launchDirection = (transform.position - Vector3.zero).normalized;
        rb.velocity = launchDirection * initialLaunchSpeed;
        hasLaunchedFromMoon = true;
        ignoredPlanet = null;
        ignoredPlanetUntil = 0f;
        lastOrbitSpeed = 0f;
        
        // ジャンプSEを再生
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEPlayerJump();
        }
    }

    void LaunchFromPlanet()
    {
        Transform previousPlanet = orbitingPlanet;
        currentState = PlayerState.Flying;

        rb.isKinematic = false;

        Vector2 launchDirection = (transform.position - orbitingPlanet.position).normalized;
        rb.velocity = launchDirection * orbitSpeed;
        orbitingPlanet = null;
        ignoredPlanet = previousPlanet;
        ignoredPlanetUntil = Time.time + samePlanetReenterDelay;
        lastOrbitSpeed = Mathf.Max(lastOrbitSpeed, orbitSpeed);
        
        // ジャンプSEを再生
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEPlayerJump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameEnded) return;
        if (collision.collider.CompareTag("Moon") && hasLaunchedFromMoon)
        {
            // 月を砕く（orbitSpeedを速度として渡す）
            MoonController moon = collision.collider.GetComponent<MoonController>();
            if (moon != null)
            {
                moon.Break(orbitSpeed);
            }
            
            EndGame();
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        float score = orbitSpeed; // orbitSpeedをスコアとして使用

        Debug.Log($"ゲーム終了: スコア {score:F2}");

        PlayerPrefs.SetFloat("Score", score); // orbitSpeedを保存

        PrepareForSceneChange();
        
        // エフェクトを見せるために少し遅延してからシーン遷移
        StartCoroutine(DelayedSceneTransition(gameClearSceneName, 1.5f));
    }

    private void GameOverOutOfScreen()
    {
        if (gameEnded) return;
        gameEnded = true;
        
        float score = orbitSpeed; // ゲームオーバー直前のorbitSpeedを保存
        Debug.Log($"{outMessage} - Final Score: {score:F2}");
        PlayerPrefs.SetFloat("Score", score); // orbitSpeedを保存
        
        PrepareForSceneChange();
        TransitionToScene(gameOverSceneName);
    }

    private void CheckOutOfScreen()
    {
        if (!enableOutOfScreenGameOver) return;
        if (gameEnded) return;
        var cam = Camera.main;
        if (cam == null) return;
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        if (vp.z < 0f) return; // カメラ裏は無視
    // Viewport(0,0)-(1,1) を中心(0.5,0.5)基準で拡大判定
    // 中心からの偏差(0.5 が境界)を正規化して判定。factor=1なら従来,1.5なら更に外側まで許容
    float fx = (vp.x - 0.5f) / 0.5f; // -1～1 が画面内
    float fy = (vp.y - 0.5f) / 0.5f;
    // マージンも併用: factorによる許容幅 + outMarginを比率に変換
    float maxAbs = screenExtentFactor + outMargin; // シンプル合成 (screenExtentFactor=1.5, outMargin=0.05 → 1.55倍)
    if (Mathf.Abs(fx) > maxAbs || Mathf.Abs(fy) > maxAbs)
        {
            GameOverOutOfScreen();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryEnterOrbit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryEnterOrbit(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("GravityField")) return;
        Transform planet = other.transform.parent;
        if (planet != null && planet == ignoredPlanet)
        {
            if (Time.time >= ignoredPlanetUntil)
            {
                ignoredPlanet = null;
                ignoredPlanetUntil = 0f;
            }
        }
    }

    private void TryEnterOrbit(Collider2D other)
    {
        if (!other.CompareTag("GravityField")) return;
        if (currentState != PlayerState.Flying) return;

        Transform planet = other.transform.parent;
        if (planet == null)
        {
            Debug.LogWarning("[PlayerController] GravityFieldの親に惑星が設定されていません。");
            return;
        }

        if (planet == ignoredPlanet)
        {
            if (Time.time < ignoredPlanetUntil)
            {
                return;
            }
            ignoredPlanet = null;
        }

        PlanetInfo planetInfo = other.GetComponent<PlanetInfo>();
        if (planetInfo == null) return;

        currentState = PlayerState.OrbitingPlanet;
        orbitingPlanet = planet;
        ignoredPlanet = null;

    float measuredSpeed = rb.velocity.magnitude;
    Vector2 center = orbitingPlanet.position;
    Vector2 fromCenter = (Vector2)transform.position - center;
        if (fromCenter.sqrMagnitude < 1e-6f)
        {
            fromCenter = rb.velocity.sqrMagnitude > 1e-4f ? rb.velocity.normalized : Vector2.right;
        }

        Vector2 radialDir = fromCenter.normalized;
        Vector2 velocityDir = rb.velocity.sqrMagnitude > 1e-4f ? rb.velocity.normalized : new Vector2(-radialDir.y, radialDir.x);
    float desiredRadius = ComputeOrbitRadius(other, orbitingPlanet);
        if (desiredRadius <= 0f)
        {
            desiredRadius = fromCenter.magnitude;
        }

        orbitRadius = Mathf.Max(desiredRadius, 0.01f);
        Vector2 targetPos = center + radialDir * orbitRadius;
        rb.position = targetPos;
        transform.position = targetPos;

        orbitAngle = Mathf.Atan2(radialDir.y, radialDir.x);

        float cross = radialDir.x * velocityDir.y - radialDir.y * velocityDir.x;
        orbitDirection = cross >= 0 ? 1 : -1;

        float boostedSpeed = measuredSpeed * planetInfo.speedMultiplier;
        if (boostedSpeed < planetInfo.minOrbitSpeed)
        {
            boostedSpeed = planetInfo.minOrbitSpeed;
        }
        if (boostedSpeed < lastOrbitSpeed)
        {
            boostedSpeed = lastOrbitSpeed;
        }

        orbitSpeed = boostedSpeed;
        orbitAngularSpeed = orbitSpeed / orbitRadius;
        float maxAngular = Mathf.PI * 2f / 0.2f;
        if (orbitAngularSpeed > maxAngular)
        {
            orbitAngularSpeed = maxAngular;
            orbitSpeed = orbitAngularSpeed * orbitRadius;
        }

        Debug.Log(orbitingPlanet.name + "の重力場に進入！ 速度が " + measuredSpeed.ToString("F1") + " → " + orbitSpeed.ToString("F1") + " に増加！");
        lastOrbitSpeed = orbitSpeed;
        ignoredPlanet = null;
        ignoredPlanetUntil = 0f;
    }

    private float ComputeOrbitRadius(Collider2D field, Transform planet)
    {
        if (field == null || planet == null) return -1f;

        float radius = -1f;

        if (field is CircleCollider2D circle)
        {
            float maxScale = Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);
            radius = circle.radius * maxScale;
        }
        else if (field is CapsuleCollider2D capsule)
        {
            float maxScale = Mathf.Max(capsule.transform.lossyScale.x, capsule.transform.lossyScale.y);
            radius = Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f * maxScale;
        }
        else if (field is BoxCollider2D box)
        {
            float maxScale = Mathf.Max(box.transform.lossyScale.x, box.transform.lossyScale.y);
            radius = Mathf.Max(box.size.x, box.size.y) * 0.5f * maxScale;
        }
        else
        {
            radius = Mathf.Max(field.bounds.extents.x, field.bounds.extents.y);
        }

        if (radius <= 0f)
        {
            return -1f;
        }

        float centerOffset = Vector2.Distance(planet.position, field.transform.position);
        return radius + centerOffset;
    }

    private void PrepareForSceneChange()
    {
        currentState = PlayerState.OnMoon;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }

    private void TransitionToScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("シーン名が設定されていないため、シーン遷移をスキップします。");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"シーン\"{sceneName}\"は Build Settings に含まれていない、またはアセットバンドルがロードされていません。File > Build Settings... で追加してください。");
            Time.timeScale = 0f;
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator DelayedSceneTransition(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        TransitionToScene(sceneName);
    }

    /// <summary>
    /// ゲームをリトライ（現在のシーンをリロード）
    /// </summary>
    private void RetryGame()
    {
        Debug.Log("[PlayerController] リトライ - シーンをリロードします");
        
        // リトライSEを再生
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySEButtonClick();
        }
        
        // Time.timeScale を戻す
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
        
        // 現在のシーンをリロード
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}