using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("砕けるエフェクト設定")]
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D moonCollider;
    
    [Header("砕ける演出の調整")]
    [SerializeField] private float minSpeedToBreak = 3f; // 砕けるための最低速度
    [SerializeField] private float maxSpeedForMaxEffect = 15f; // 最大演出の速度
    [SerializeField] private float fadeOutDuration = 0.5f; // フェードアウト時間

    private bool isBroken = false;

    void Start()
    {
        // コンポーネントを自動取得
        if (breakEffect == null)
            breakEffect = GetComponent<ParticleSystem>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (moonCollider == null)
            moonCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isBroken)
        {
            // Z軸を中心に回転させる
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 月を砕く（速度に応じて演出を変える）
    /// </summary>
    /// <param name="impactSpeed">衝突時の速度</param>
    public void Break(float impactSpeed)
    {
        if (isBroken) return;
        
        isBroken = true;
        
        // 速度に応じて演出の強度を計算（0.0～1.0）
        float intensity = Mathf.Clamp01((impactSpeed - minSpeedToBreak) / (maxSpeedForMaxEffect - minSpeedToBreak));
        
        // パーティクルエフェクトを再生
        if (breakEffect != null)
        {
            // 速度に応じてパーティクルの飛び方を調整
            var main = breakEffect.main;
            main.startSpeed = 5f + intensity * 15f; // 速度が高いほど破片が速く飛ぶ
            
            // 速度に応じて破片の数を調整
            var emission = breakEffect.emission;
            if (emission.burstCount > 0)
            {
                ParticleSystem.Burst burst = emission.GetBurst(0);
                burst.count = Mathf.RoundToInt(50 + intensity * 150); // 速度が高いほど破片が多い
                emission.SetBurst(0, burst);
            }
            
            breakEffect.Play();
            
            Debug.Log($"[MoonController] パーティクル再生 - 速度: {impactSpeed:F2}, 強度: {intensity:P0}, 破片数: {50 + intensity * 150:F0}");
        }
        else
        {
            Debug.LogWarning("[MoonController] ParticleSystem が設定されていません");
        }
        
        // 月の見た目を徐々に消す
        if (spriteRenderer != null)
        {
            StartCoroutine(FadeOut());
        }
        
        // 当たり判定を無効化
        if (moonCollider != null)
        {
            moonCollider.enabled = false;
        }
        
        Debug.Log($"[MoonController] 月が砕けた！衝撃速度: {impactSpeed:F2}, 演出強度: {intensity:P0}");
    }

    /// <summary>
    /// 月をフェードアウトさせる
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color original = spriteRenderer.color;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);
            spriteRenderer.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }
        
        // 完全に透明にして非表示
        spriteRenderer.enabled = false;
    }

    /// <summary>
    /// 月をリセット（リプレイ用・デバッグ用）
    /// </summary>
    public void ResetMoon()
    {
        isBroken = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            Color c = spriteRenderer.color;
            spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
        }
        
        if (moonCollider != null)
        {
            moonCollider.enabled = true;
        }
        
        if (breakEffect != null)
        {
            breakEffect.Stop();
            breakEffect.Clear();
        }
        
        Debug.Log("[MoonController] 月をリセットしました");
    }
}