# AppDataHelper(一時情報保持)

アプリケーションを終了したら保持されない一時データを保持するためのクラス。

### 設定項目の追加

プロパティを追加する。

```csharp
public string LoginUser { get; set; }
```

### 設定値の書き換え

```csharp
Settinghelper.Instance.IsEnabledSettingPasswordLock = false;
```

### 設定値の読込

```csharp
var val = Settinghelper.Instance.IsEnabledSettingPasswordLock;
```