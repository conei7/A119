using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip bgmTitle;
    [SerializeField] private AudioClip bgmGame;
    [SerializeField] private AudioClip bgmGameOver;
    [SerializeField] private AudioClip bgmGameClear;
    
    [Header("BGM Loop Settings (Optional)")]
    [SerializeField] private AudioClip bgmGameClearIntro; // イントロ部分（1回だけ再生）
    [SerializeField] private AudioClip bgmGameClearLoop;  // ループ部分（無限ループ）

    [Header("Auto BGM Settings")]
    [SerializeField] private bool enableAutoBGM = true;
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string gameOverSceneName = "GameOver";
    [SerializeField] private string gameClearSceneName = "GameClear";

    [Header("SE Clips")]
    [SerializeField] private AudioClip seButtonClick;
    [SerializeField] private AudioClip sePlayerJump;
    [SerializeField] private AudioClip seMoonExplosion; // 月の爆発音

    [Header("Volume Settings")]
    [SerializeField] private float bgmVolume = 0.8f;
    [SerializeField] private float seVolume = 0.8f;

    private const string BGM_VOLUME_PARAM = "VolumeParamBGM";
    private const string SE_VOLUME_PARAM = "MyExposedParam";
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
            
            // シーン変更時のイベントを登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVolumeSettings();
        
        // 初回のシーンでBGMを再生
        if (enableAutoBGM)
        {
            PlayBGMForCurrentScene();
        }
    }

    private void OnDestroy()
    {
        // イベントを解除
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// シーンが読み込まれたときに呼ばれる
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // イントロ→ループのコルーチンをキャンセル
        StopAllCoroutines();
        
        if (enableAutoBGM)
        {
            PlayBGMForCurrentScene();
        }
    }

    /// <summary>
    /// 現在のシーン名に応じてBGMを再生
    /// </summary>
    private void PlayBGMForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == titleSceneName)
        {
            PlayBGMTitle();
        }
        else if (currentSceneName == gameSceneName)
        {
            PlayBGMGame();
        }
        else if (currentSceneName == gameOverSceneName)
        {
            PlayBGMGameOver();
        }
        else if (currentSceneName == gameClearSceneName)
        {
            PlayBGMGameClear();
        }
    }

    /// <summary>
    /// 保存された音量設定を読み込む
    /// </summary>
    private void LoadVolumeSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.8f);
        seVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.8f);
    }

    /// <summary>
    /// 音量設定をAudioMixerに適用する
    /// </summary>
    private void ApplyVolumeSettings()
    {
        SetBGMVolume(bgmVolume);
        SetSEVolume(seVolume);
    }

    /// <summary>
    /// BGMの音量を設定する (0.0 ~ 1.0)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        float db = VolumeToDecibel(bgmVolume);
        audioMixer.SetFloat(BGM_VOLUME_PARAM, db);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SEの音量を設定する (0.0 ~ 1.0)
    /// </summary>
    public void SetSEVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        float db = VolumeToDecibel(seVolume);
        audioMixer.SetFloat(SE_VOLUME_PARAM, db);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, seVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 音量(0~1)をデシベル(-80~0)に変換
    /// </summary>
    private float VolumeToDecibel(float volume)
    {
        if (volume <= 0f)
        {
            return -80f;
        }
        return Mathf.Log10(volume) * 20f;
    }

    /// <summary>
    /// 現在のBGM音量を取得 (0.0 ~ 1.0)
    /// </summary>
    public float GetBGMVolume()
    {
        return bgmVolume;
    }

    /// <summary>
    /// 現在のSE音量を取得 (0.0 ~ 1.0)
    /// </summary>
    public float GetSEVolume()
    {
        return seVolume;
    }

    // ==================== BGM再生メソッド ====================

    /// <summary>
    /// タイトルBGMを再生
    /// </summary>
    public void PlayBGMTitle()
    {
        PlayBGM(bgmTitle);
    }

    /// <summary>
    /// ゲームBGMを再生
    /// </summary>
    public void PlayBGMGame()
    {
        PlayBGM(bgmGame);
    }

    /// <summary>
    /// ゲームオーバーBGMを再生
    /// </summary>
    public void PlayBGMGameOver()
    {
        PlayBGM(bgmGameOver);
    }

    /// <summary>
    /// ゲームクリアBGMを再生
    /// イントロ+ループ設定がある場合はそちらを優先
    /// </summary>
    public void PlayBGMGameClear()
    {
        // イントロ+ループが設定されている場合
        if (bgmGameClearIntro != null && bgmGameClearLoop != null)
        {
            PlayBGMWithIntro(bgmGameClearIntro, bgmGameClearLoop);
        }
        else
        {
            // 通常のループ再生
            PlayBGM(bgmGameClear);
        }
    }

    /// <summary>
    /// BGMを再生する(内部メソッド)
    /// </summary>
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGM clip is null!");
            return;
        }

        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            // 同じBGMが既に再生中の場合は何もしない
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// イントロ→ループの順でBGMを再生
    /// </summary>
    private void PlayBGMWithIntro(AudioClip intro, AudioClip loop)
    {
        if (intro == null || loop == null)
        {
            Debug.LogWarning("Intro or Loop clip is null!");
            return;
        }

        // 既に同じイントロが再生中なら何もしない
        if (bgmSource.clip == intro && bgmSource.isPlaying)
        {
            return;
        }

        // 既存のコルーチンをすべてキャンセル
        StopAllCoroutines();
        
        // 新しいイントロ→ループを開始
        StartCoroutine(PlayIntroThenLoop(intro, loop));
    }

    /// <summary>
    /// イントロ再生後、ループに切り替えるコルーチン
    /// </summary>
    private IEnumerator PlayIntroThenLoop(AudioClip intro, AudioClip loop)
    {
        // イントロを1回だけ再生
        bgmSource.clip = intro;
        bgmSource.loop = false;
        bgmSource.Play();

        // イントロの再生時間だけ待機
        float elapsedTime = 0f;
        while (elapsedTime < intro.length)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
            
            // 途中で停止された場合は処理を中断
            if (!bgmSource.isPlaying || bgmSource.clip != intro)
            {
                yield break;
            }
        }

        // ループ部分に切り替え
        bgmSource.clip = loop;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// BGMをフェードアウト
    /// </summary>
    public void FadeOutBGM(float duration = 1f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    // ==================== SE再生メソッド ====================

    /// <summary>
    /// ボタンクリックSEを再生
    /// </summary>
    public void PlaySEButtonClick()
    {
        PlaySE(seButtonClick);
    }

    /// <summary>
    /// プレイヤージャンプSEを再生
    /// </summary>
    public void PlaySEPlayerJump()
    {
        PlaySE(sePlayerJump);
    }

    /// <summary>
    /// 月の爆発SEを再生
    /// </summary>
    public void PlaySEMoonExplosion()
    {
        PlaySE(seMoonExplosion);
    }

    /// <summary>
    /// SEを再生する(内部メソッド)
    /// </summary>
    private void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SE clip is null!");
            return;
        }

        seSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 指定したAudioClipでSEを再生(カスタム用)
    /// </summary>
    public void PlaySECustom(AudioClip clip)
    {
        PlaySE(clip);
    }
}
