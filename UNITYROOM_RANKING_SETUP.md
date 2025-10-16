# unityroomランキング送信が動作しない場合のチェックリスト

## 🎯 現在の実装状態

✅ **PlayerController.cs の EndGame() でスコア送信を実装済み**
- クリア時（月に戻った時）に自動的に `SendScoreToUnityroom()` が呼ばれます
- ScoreSubmitUI からは送信されません（重複防止）

---

## 🔍 送信されない原因と解決方法

### 原因1: UnityroomApiClientが配置されていない

#### 確認方法
1. **タイトルシーンまたは最初のシーン**を開く
2. Hierarchyで「UnityroomApiClient」を検索
3. 見つからない場合は以下を実行

#### 解決方法：UnityroomApiClientを追加

1. **タイトルシーンを開く**（または最初に読み込まれるシーン）

2. **Hierarchy で右クリック → Create Empty**

3. 名前を `UnityroomApiClient` に変更

4. **Inspector で Add Component をクリック**

5. 検索欄に `UnityroomApiClient` と入力

6. 表示された `Unityroom Api Client` スクリプトを追加

7. **UnityroomApiClient コンポーネントの設定:**
   - 特に設定は不要（デフォルトでOK）
   - `DontDestroyOnLoad` が自動的に設定されます

8. **Ctrl+S でシーンを保存**

---

### 原因2: WebGLビルドでない

#### 確認方法
- エディタで実行している場合、送信は**スキップされます**
- これは正常な動作です

#### 解決方法：WebGLでビルドしてテスト

1. **File → Build Settings**

2. **Platform** で `WebGL` を選択

3. **Switch Platform** をクリック（まだの場合）

4. **Build And Run** でビルド

5. unityroomにアップロード

6. unityroomで実際にプレイしてテスト

---

### 原因3: スコアボードIDが間違っている

#### 確認方法
unityroom の「ゲーム編集」→「ランキング」で設定したスコアボードIDを確認

#### 解決方法

1. unityroomのゲーム編集ページを開く

2. 「ランキング」タブで**スコアボードID**を確認
   - 通常は `1` ですが、2つ目のランキングを作った場合は `2` など

3. PlayerController.cs の SendScoreToUnityroom() で以下を修正:

```csharp
// スコアボードID 1 の部分を変更
UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);
                                    // ↑ この数字を変更
```

---

### 原因4: ScoreboardWriteModeが間違っている

#### 確認方法
unityroomのランキング設定で「降順」か「昇順」か確認

#### 解決方法

**降順（スコアが高いほど上位）の場合:**
```csharp
UnityroomApiClient.Instance.SendScore(1, score, unityroom.Api.ScoreboardWriteMode.HighScoreDesc);
```

**昇順（スコアが低いほど上位）の場合:**
```csharp
UnityroomApiClient.Instance.SendScore(1, score, unityroom.Api.ScoreboardWriteMode.HighScoreAsc);
```

A119ゲームは**速度が高いほど良い**ので `HighScoreDesc` が正しいです。

---

## 🧪 動作確認方法

### エディタでの確認（送信はされない）

1. ゲームシーンでプレイ
2. クリアする（月に戻る）
3. **Console** で以下のログを確認:
   ```
   [SendScoreToUnityroom] スコア送信処理開始: 45.23
   [SendScoreToUnityroom] Editor/非WebGL環境のため送信スキップ: 45.23
   ```

### WebGLビルドでの確認（実際に送信される）

1. **WebGLでビルド**
2. unityroomにアップロード
3. ブラウザでプレイ
4. クリアする
5. **ブラウザのConsole**（F12キー → Console）で以下を確認:
   ```
   [SendScoreToUnityroom] スコア送信処理開始: 45.23
   [SendScoreToUnityroom] WebGLビルドで実行中
   [SendScoreToUnityroom] UnityroomApiClient.Instance が見つかりました
   [SendScoreToUnityroom] unityroomにスコアを送信しました: 45.23
   ```

6. **unityroomのランキングページ**でスコアが登録されているか確認

---

## 📋 完全チェックリスト

### Unity側の設定

- [ ] タイトルシーンに `UnityroomApiClient` GameObject が配置されている
- [ ] UnityroomApiClient に `Unityroom Api Client` スクリプトがアタッチされている
- [ ] ゲームシーンからは UnityroomApiClient を削除している（重複防止）
- [ ] Platform が WebGL に設定されている

### コード側の確認

- [ ] PlayerController.cs の EndGame() で SendScoreToUnityroom() を呼んでいる
- [ ] スコアボードIDが正しい（通常は 1）
- [ ] ScoreboardWriteMode が正しい（高スコアが上位なら HighScoreDesc）

### unityroom側の設定

- [ ] ゲーム編集ページで「ランキング」が有効になっている
- [ ] スコアボードが作成されている
- [ ] スコアの並び順が「降順」になっている（高い方が上位）

---

## 🐛 トラブルシューティング

### Q1: Consoleに「UnityroomApiClient.Instanceが存在しません」と表示される

**原因:** UnityroomApiClient が配置されていない

**解決方法:**
1. タイトルシーンに UnityroomApiClient を追加（上記参照）
2. シーンを保存
3. ビルドし直す

### Q2: エディタで実行しても送信されない

**回答:** これは正常です。

- エディタでは送信されません（`#if UNITY_WEBGL && !UNITY_EDITOR` のため）
- WebGLでビルドしてunityroomにアップロードすれば送信されます

### Q3: WebGLビルドでも送信されない

**確認事項:**
1. ブラウザのConsole（F12キー）でエラーを確認
2. UnityroomApiClient が配置されているか確認
3. unityroomのランキング設定を確認

**デバッグ方法:**
ブラウザのConsoleで以下を実行:
```javascript
// UnityroomApiClientが存在するか確認
console.log("UnityroomApiClient exists?", typeof unityroom !== 'undefined');
```

### Q4: 「複数のUnityroomApiClientが見つかりました」エラー

**原因:** 複数のシーンに UnityroomApiClient が配置されている

**解決方法:**
1. 各シーンを開いて UnityroomApiClient を検索
2. タイトルシーン以外から削除
3. ビルドし直す

---

## ✅ 正しい構成

```
[タイトルシーン]
  └─ UnityroomApiClient  ← ここに1つだけ

[ゲームシーン]
  └─ GamePlayTracker
  └─ PlayerController
  └─ (UnityroomApiClientは配置しない)

[GameClearシーン]
  └─ ScoreSubmitUI
  └─ (UnityroomApiClientは配置しない)
  └─ (名前入力のみ、unityroom送信はしない)
```

---

## 🎯 最終確認手順

1. ✅ タイトルシーンに UnityroomApiClient を追加
2. ✅ ゲームシーンから UnityroomApiClient を削除
3. ✅ WebGLでビルド
4. ✅ unityroomにアップロード
5. ✅ ブラウザでプレイ＆クリア
6. ✅ ブラウザのConsole（F12）でログ確認
7. ✅ unityroomのランキングページで確認

これで確実にクリア時にランキング送信されます！
