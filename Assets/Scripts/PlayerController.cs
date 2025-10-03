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
    private float orbitSpeed;

    private float orbitRadius;
    private float orbitAngle;
    private float orbitAngularSpeed;
    private int orbitDirection = 1;

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
    }

    void LaunchFromPlanet()
    {
        currentState = PlayerState.Flying;

        rb.isKinematic = false;

        Vector2 launchDirection = (transform.position - orbitingPlanet.position).normalized;
        rb.velocity = launchDirection * orbitSpeed;
        orbitingPlanet = null;
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
        if (other.CompareTag("GravityField") && currentState == PlayerState.Flying)
        {
            PlanetInfo planetInfo = other.GetComponent<PlanetInfo>();
            if (planetInfo == null) return;

            currentState = PlayerState.OrbitingPlanet;
            orbitingPlanet = other.transform.parent;

            float currentSpeed = rb.velocity.magnitude;
            orbitSpeed = currentSpeed * planetInfo.speedMultiplier;

            orbitRadius = Vector2.Distance(transform.position, orbitingPlanet.position);
            if (orbitRadius < 0.01f) orbitRadius = 0.01f;

            Vector2 localPos = (Vector2)(transform.position - orbitingPlanet.position);
            orbitAngle = Mathf.Atan2(localPos.y, localPos.x);

            Vector2 radialDir = localPos.normalized;
            Vector2 velocityDir = rb.velocity.normalized;
            float cross = radialDir.x * velocityDir.y - radialDir.y * velocityDir.x;
            orbitDirection = cross >= 0 ? 1 : -1;

            orbitAngularSpeed = orbitSpeed / orbitRadius;
            float maxAngular = Mathf.PI * 2f / 0.2f;
            if (orbitAngularSpeed > maxAngular)
            {
                orbitAngularSpeed = maxAngular;
                orbitSpeed = orbitAngularSpeed * orbitRadius;
            }

            Debug.Log(orbitingPlanet.name + "の重力場に進入！ 速度が " + currentSpeed.ToString("F1") + " → " + orbitSpeed.ToString("F1") + " に増加！");
        }
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
}