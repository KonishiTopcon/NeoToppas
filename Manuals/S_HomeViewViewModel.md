# HomeView/ViewModel

Home画面には、アプリ名表示・バージョン表示、Info画面・設定画面の表示ボタン、ログアウト、終了ボタンの他、メニューボタンの追加機能がある。

![image.png](assets/image%203.png)

## アプリ名表示

```csharp
public ReactivePropertySlim<string> AppText { get; } = new ReactivePropertySlim<string>();
private string GetApplicationName()
{
    var appName = Assembly.GetExecutingAssembly().GetName().Name;
    var point = appName.IndexOf(".");
    if (point <= 0)
    {
        return appName;
    }
    return appName.Substring(0, point);
}
```

## バージョン表示

```csharp
public ReactivePropertySlim<string> VersionNo { get; } = new ReactivePropertySlim<string>("Ver " + Assembly.GetExecutingAssembly().GetName().Version);
```

## Info画面(ライセンス表示)

![image.png](assets/image%204.png)

## メニューボタンの追加

```csharp
// 表示メニューボタンの設定
MenuButtons.Add(new MenuButtonEntity("テスト", () =>
{
    _message.ShowSnackbar("テスト");
}));
```