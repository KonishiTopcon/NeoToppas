# TransitionService(画面遷移)

同じウインドウで画面を変更する画面遷移(Navigation)を行いたい場合には、ContentNavigationServiceを使用します。

## パラメータ無しで表示

### TransitionServiceの受け取り

呼び出す側のViewModelのコンストラクタでTransitionServiceを受け取り、プライベートフィールド変数で保持します。

```csharp
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly TransitionService_transition;

    public event PropertyChangedEventHandler? PropertyChanged;
    public LoginViewModel(TransitionService transitionService)
    {
        _transition= transitionService;
    }
}
```

### 画面遷移の実行

TransitionServiceのNavigateTo<TView>で表示したいViewを指定し実行することで、指定の画面に遷移します。

```csharp
_transition.NavigateTo<HomeView>();
```

※パラメータが無い場合は、表示側の設定は不要です

---

## パラメータを渡して表示

### パラメータ受け取り設定

表示される側のViewModelはITransitionAwareを継承し、OnNavigatedToで受け取る型に合わせて型変換し、受け取ったデータに対して処理を行うプログラムを記述します。

```csharp
public class LicenseViewModel : INotifyPropertyChanged, ITransitionAware
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public LicenseViewModel(ContentNavigationService contentNavigation)
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        //受け取ったデータの処理
        if (parameter is string message)
        {
            var receivedMessage = message;
        }
    }
}
```

### TransitionServiceの受け取り

呼び出す側のViewModelのコンストラクタでTransitionServiceを受け取り、プライベートフィールド変数で保持します。

```csharp
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly TransitionService_transition;

    public event PropertyChangedEventHandler? PropertyChanged;
    public LoginViewModel(TransitionService transitionService)
    {
        _transition= transitionService;
    }
}
```

### 画面遷移の実行

NavigateTo<TView>で表示したいViewを指定し、パラメータを渡して実行します。

```csharp
_transition.NavigateTo<HomeView>("test");
```

---

## DIからViewを受け取って遷移する場合

※循環参照が起きる可能性があるため、参考記載です

### TransitionServiceの受け取り

呼び出す側のViewModelのコンストラクタでTransitionService を受け取り、プライベートフィールド変数で保持します。

```csharp
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly TransitionService _transition;

    public event PropertyChangedEventHandler? PropertyChanged;
    public LoginViewModel(TransitionService transitionService)
    {
        _transition= transitionService;
    }
}
```

### Viewの受け取り

TransitionService と同様に、ViewもコンストラクタでDIから受け取り、プライベートフィールドで保持します。

```csharp
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly TransitionService _transition;
    private readonly HomeView _view;

    public event PropertyChangedEventHandler? PropertyChanged;
    public LoginViewModel(TransitionService transitionService, HomeView view)
    {
        _transition = contentNavigation;
        _view = view;
    }
}
```

### 画面遷移の実行

NavigateTo<TView>で受け取ったViewを指定して実行します。

```csharp
_transition.NavigateTo(_view);
```