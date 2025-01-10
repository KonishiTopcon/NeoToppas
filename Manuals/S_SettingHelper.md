# SettingHelper(設定保存)

アプリケーションを閉じても保持したいデータを保存するためのクラス。

データはexeと同じディレクトリにsettings.configファイルで保存され、内容はxmlで記述されている。

### 設定項目の追加

プロパティを追加する。

```csharp
public string SettingItem { get; set; } = string.Empty;
```

### 設定値の書き換え

```csharp
Settinghelper.Instance.IsEnabledSettingPasswordLock = false;
```

### 設定値の読込

```csharp
var val = Settinghelper.Instance.IsEnabledSettingPasswordLock;
```

### 設定保存(ファイル書き出し)

現在プロパティに設定されている値をファイルに出力する。

```csharp
SettingSave();
```

### ファイル読み込み

保存されているファイルを読み込み、現在のプロパティの値に上書きする。

```csharp
SettingLoad();
```

### ファイル削除

保存されている設定ファイルを削除する。

```csharp
SettingDelete();
```