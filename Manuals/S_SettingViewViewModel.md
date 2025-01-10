# SettingView/ViewModel

![image.png](assets/image%205.png)

SettingGroupsに項目を追加していくことで、その内容が画面に描画される。

### アイテム設定

SettingItemEntityによって表示される項目を作成する。

```csharp
var settings = new List<ISettingItem>();
settings.Add(new SettingItemEntity<設定値の型>("項目名", 初期値, 表示形式)
.SetSaveAction(entity =>
{
    //データを保存する処理
    //entity.SettingItem.Valueで項目の値を取得可能
}));
```

ItemType

| 指定値 | 内容 | 使用例 |
| --- | --- | --- |
| TextBox | テキスト入力の項目 | settings.Add(new SettingItemEntity<string>("テキスト", string.Empty)
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})); |
| ToggleButton | 有効・無効の項目 | settings.Add(new SettingItemEntity<bool>("有効無効", false, ItemType.ToggleButton)
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})); |
| ComboBox | 単一選択の項目 | settings.Add(new SettingItemEntity<string>("単一選択", "b", ItemType.ComboBox, new ReactiveCollection<string>() { "a", "b", "c", "d" })
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})); |
| DirectorySelect | フォルダ選択項目 | settings.Add(new SettingItemEntity<string>("フォルダ選択", string.Empty, ItemType.DirectorySelect)
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})); |
| FileSelect | ファイル選択項目 | settings.Add(new SettingItemEntity<string>("ファイル選択", string.Empty, ItemType.FileSelect)
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})); |
| ButtonOnly | Actionを実行する項目 | settings.Add(new SettingItemEntity<int>("ボタン", 0, ItemType.ButtonOnly)
.SetButtonAction(() =>
{
// ボタン押下時の処理
})); |

Validation

| 項目 | 内容 | 使用例 |
| --- | --- | --- |
| SetStringMinLengthValidation | 最小テキスト文字数 | settings.Add(new SettingItemEntity<string>("テキスト", "aaaaa")
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})
.SetStringMinLengthValidation(5)); |
| SetStringRequiredValidation | 必須入力 | settings.Add(new SettingItemEntity<string>("テキスト", "aaaaa")
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})
.SetStringRequiredValidation()); |
| SetAlphanumericOrSymbolValidation | 半角英数字入力制限 | settings.Add(new SettingItemEntity<string>("テキスト", "aaaaa")
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})
.SetAlphanumericOrSymbolValidation()); |
| SetNumericalRangeValidation | 数値範囲制限 | settings.Add(new SettingItemEntity<string>("テキスト", "1")
.SetSaveAction(entity =>
{
var setting = entity.SettingItem.Value;
})
.SetNumericalRangeValidation(10, 20)); |

※Validationを組み合わせることも可能

```csharp
settings.Add(new SettingItemEntity<string>("テキスト", string.Empty)
.SetSaveAction(entity =>
{
    var setting = entity.SettingItem.Value;
})
.SetStringMinLengthValidation(5)
.SetStringRequiredValidation());
```

### グループ設定

SettingItemEntityを組み合わせてグループ設定を作成する。

```csharp
new SettingGroupEntity("見出し", List<SettingItem>, 表示を広げておくか);
```

```csharp
SettingGroups.Add(CreateGeneralSettings());

private SettingGroupEntity CreateGeneralSettings()
{
    var settings = new List<ISettingItem>();
    settings.Add(new SettingItemEntity<string>("設定項目1", "aaaaa")
    .SetSaveAction(entity =>
    {
        var setting = entity.SettingItem.Value;
    })
    .SetStringMinLengthValidation(5));

    settings.Add(new SettingItemEntity<bool>("設定項目2", true, ItemType.ToggleButton)
    .SetSaveAction(entity =>
    {
        var setting = entity.SettingItem.Value;
    }));

    settings.Add(new SettingItemEntity<bool>("設定項目2", false, ItemType.ToggleButton)
    .SetSaveAction(entity =>
    {
        var setting = entity.SettingItem.Value;
    }));

    settings.Add(new SettingItemEntity<string>("設定項目3", "b", ItemType.ComboBox, new ReactiveCollection<string>() { "a", "b", "c", "d" })
    .SetSaveAction(entity =>
    {
        var setting = entity.SettingItem.Value;
    }));

    return new SettingGroupEntity("一般設定", settings, true);
}
```

## パスワード設定

設定画面をパスワード保護する機能は標準で実装済み。

※パスワードはHash + Saltで保護しているが、settings.configを書き換えることで簡単に突破できるため、そのままでは完全な保護はできない(非常時の抜け道)