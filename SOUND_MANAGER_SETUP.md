# サウンドマネージャー セットアップガイド

## 概要
このサウンドマネージャーシステムは、UnityのAudioMixerを使用して、シーン間で音量設定を保持し、BGMとSEを一元管理します。

## 作成されたスクリプト

### 1. SoundManager.cs
- シングルトンパターンでシーン間で永続化
- BGM・SEの音量管理と再生制御
- PlayerPrefsで音量設定を保存

### 2. VolumeSettings.cs
- UIスライダーと連携して音量を調整
- タイトル以外のシーンで使用

---

## セットアップ手順

### ステップ1: AudioMixerの設定確認

AudioMixer (`Assets/NewAudioMixer.mixer`) は既に以下のように設定されています:
- **Master** グループ
  - **BGM** グループ (Exposed Parameter: `VolumeParamBGM`)
  - **SE** グループ (Exposed Parameter: `MyExposedParam`)

**確認事項:**
1. Unity エディタで `Assets/NewAudioMixer.mixer` を開く
2. BGMグループのVolume パラメータを右クリック → "Expose 'Volume (of BGM)' to script" を選択
3. SEグループのVolume パラメータも同様に設定
4. Inspector上部の「Exposed Parameters」で名前を確認:
   - BGM: `VolumeParamBGM`
   - SE: `MyExposedParam`

### ステップ2: SoundManagerオブジェクトの作成

1. **空のGameObjectを作成:**
   - Hierarchy で右クリック → Create Empty
   - 名前を `SoundManager` に変更

2. **SoundManagerコンポーネントをアタッチ:**
   - `SoundManager` オブジェクトを選択
   - Add Component → `SoundManager` スクリプトを追加

3. **AudioSourceコンポーネントを2つ追加:**
   - Add Component → Audio Source (BGM用)
   - Add Component → Audio Source (SE用)

4. **SoundManagerコンポーネントの設定:**
   - **Audio Mixer:** `NewAudioMixer` をドラッグ&ドロップ
   - **Bgm Source:** 1つ目のAudio Sourceをドラッグ&ドロップ
   - **Se Source:** 2つ目のAudio Sourceをドラッグ&ドロップ
   - **BGM Clips:**
     - `Bgm Title`: タイトルシーン用のBGMファイル
     - `Bgm Game`: ゲームシーン用のBGMファイル
     - `Bgm Game Over`: ゲームオーバーシーン用のBGMファイル
     - `Bgm Game Clear`: ゲームクリアシーン用のBGMファイル
   - **SE Clips:**
     - `Se Button Click`: ボタンクリック時のSEファイル
     - `Se Player Jump`: プレイヤージャンプ時のSEファイル

5. **Audio Sourceの設定:**
   - BGM用のAudio Source:
     - Output: `NewAudioMixer` → `BGM` グループに設定
     - Play On Awake: OFF
     - Loop: OFF (スクリプトで制御)
   - SE用のAudio Source:
     - Output: `NewAudioMixer` → `SE` グループに設定
     - Play On Awake: OFF
     - Loop: OFF

6. **Prefabとして保存:**
   - `SoundManager` オブジェクトを `Assets/Prefabs` フォルダにドラッグしてPrefab化

### ステップ3: 最初のシーンに配置

1. タイトルシーンを開く
2. `SoundManager` Prefabをシーンに配置
3. これで全シーンで使用可能になります(DontDestroyOnLoadで永続化)

### ステップ4: 音量設定UIの作成(タイトル以外のシーン)

1. **Canvas作成:**
   - Hierarchy → 右クリック → UI → Canvas

2. **BGMスライダー作成:**
   - Canvas を右クリック → UI → Slider
   - 名前を `BGM Slider` に変更
   - 位置を調整

3. **SEスライダー作成:**
   - Canvas を右クリック → UI → Slider
   - 名前を `SE Slider` に変更
   - 位置を調整

4. **Textラベル追加(オプション):**
   - 各スライダーに「BGM」「SE」のラベルを追加

5. **VolumeSettingsコンポーネントをアタッチ:**
   - Canvas(または空のGameObject)を選択
   - Add Component → `VolumeSettings` スクリプトを追加
   - **Bgm Slider:** BGM Sliderをドラッグ&ドロップ
   - **Se Slider:** SE Sliderをドラッグ&ドロップ

---

## 使用方法

### BGMの再生

各シーンの開始時にBGMを再生する例:

```csharp
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    void Start()
    {
        // タイトルBGMを再生
        SoundManager.Instance.PlayBGMTitle();
    }
}
```

```csharp
using UnityEngine;

public class GameScene : MonoBehaviour
{
    void Start()
    {
        // ゲームBGMを再生
        SoundManager.Instance.PlayBGMGame();
    }
}
```

```csharp
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    void Start()
    {
        // ゲームオーバーBGMを再生
        SoundManager.Instance.PlayBGMGameOver();
    }
}
```

```csharp
using UnityEngine;

public class GameClearScene : MonoBehaviour
{
    void Start()
    {
        // ゲームクリアBGMを再生
        SoundManager.Instance.PlayBGMGameClear();
    }
}
```

### SEの再生

#### ボタンクリック時のSE

```csharp
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // ボタンクリックSEを再生
        SoundManager.Instance.PlaySEButtonClick();
    }
}
```

#### プレイヤージャンプ時のSE

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        // ジャンプ処理
        // ...

        // ジャンプSEを再生
        SoundManager.Instance.PlaySEPlayerJump();
    }
}
```

### 音量の取得・設定

```csharp
// 音量を取得
float bgmVolume = SoundManager.Instance.GetBGMVolume();
float seVolume = SoundManager.Instance.GetSEVolume();

// 音量を設定 (0.0 ~ 1.0)
SoundManager.Instance.SetBGMVolume(0.5f);
SoundManager.Instance.SetSEVolume(0.8f);
```

### その他の機能

```csharp
// BGMを停止
SoundManager.Instance.StopBGM();

// BGMをフェードアウト(1秒かけて)
SoundManager.Instance.FadeOutBGM(1f);

// カスタムSEを再生
AudioClip customClip = Resources.Load<AudioClip>("MySE");
SoundManager.Instance.PlaySECustom(customClip);
```

---

## 仕組み

### シングルトンパターン
- `SoundManager.Instance` でどこからでもアクセス可能
- `DontDestroyOnLoad()` でシーン遷移時も破棄されない
- 重複したインスタンスは自動的に削除される

### 音量の保存
- `PlayerPrefs` を使用して音量設定を保存
- ゲームを再起動しても設定が保持される
- キー名:
  - BGM: `"BGMVolume"`
  - SE: `"SEVolume"`

### AudioMixer連携
- スライダーの値(0~1)をデシベル(-80~0)に変換
- AudioMixerのExposed Parameterを通じて音量を制御
- リアルタイムで音量が反映される

---

## トラブルシューティング

### BGM/SEが再生されない
- AudioClipが正しく設定されているか確認
- AudioSourceのOutputがAudioMixerの適切なグループに設定されているか確認
- SoundManagerがシーンに存在するか確認

### 音量が変わらない
- AudioMixerのExposed Parametersが正しく設定されているか確認
- パラメータ名が一致しているか確認:
  - BGM: `VolumeParamBGM`
  - SE: `MyExposedParam`

### シーン遷移後にSoundManagerが消える
- SoundManagerのAwake()で`DontDestroyOnLoad(gameObject)`が呼ばれているか確認
- 最初のシーン(タイトルシーンなど)にSoundManagerが配置されているか確認

### スライダーが動かない
- VolumeSettingsスクリプトにスライダーが正しく設定されているか確認
- SoundManagerがシーンに存在するか確認

---

## カスタマイズ

### BGM/SEを追加する場合

1. `SoundManager.cs` にAudioClipのフィールドを追加
2. 再生メソッドを追加
3. Inspectorで新しいクリップを設定

例:
```csharp
[Header("SE Clips")]
[SerializeField] private AudioClip seNewSound;

public void PlaySENewSound()
{
    PlaySE(seNewSound);
}
```

### 音量スライダーのデザイン変更

- Sliderの `Fill Area`, `Handle` をカスタマイズ
- TextMeshProを使ってラベルを追加
- 数値表示を追加する場合は `VolumeSettings.cs` を拡張

---

## まとめ

このサウンドマネージャーシステムで以下が実現できます:
✅ シーン間で音量設定を引き継ぎ
✅ BGMとSEを一元管理
✅ スライダーUIで簡単に音量調整
✅ PlayerPrefsで設定を永続化
✅ AudioMixerで高品質な音声制御

ご質問があれば、お気軽にお尋ねください!
