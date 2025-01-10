# DialogService(ダイアログ表示)

新しいWIndow上に指定のViewを表示したい場合には、DialogServiceを使用します。パラメータの有無にかかわらず、表示側のViewModelではIDialogAwareを継承する必要があります。

### 表示側ViewModelの準備

IDialogAwareを継承します。OnDialogOpenedで受け取ったパラメータの設定をおこないますが、パラメータを受け取る必要が無い場合には処理を書く必要はありません。

```csharp
public class LicenseViewModel : INotifyPropertyChanged, IDialogAware
{
    public event EventHandler<bool> RequestClose;
    public ReactiveCommand HomeButton { get; }
    
    public LicenseViewModel()
    {
        HomeButton = new ReactiveCommand().WithSubscribe(() =>
        {
		        RequestClose?.Invoke(this, true);
        });
        
				public void OnDialogOpened(object parameter)
				{
				    //受け取ったデータの処理
				    if (parameter is string message)
				    {
				        var receivedMessage = message;
				    }
				}
    }
}
```

### DialogServiceの受け取り

呼び出し側ViewModelのコンストラクタでDialogService を受け取り、プライベートフィールド変数で保持します。

```csharp
private readonly DialogService _dialog;

public HomeViewModel(DialogService dialogService)
{
    _dialog = dialogService;
    InformationButton = new ReactiveCommand().WithSubscribe(async () =>
    {
        _dialog.ShowDialog<LicenseView>();
    });
}
```

### Dialog表示の実行

ShowDialog<TView>で表示したいViewを指定し実行することで、Dialogが表示されます。表示したWindowしか操作できなくする場合にはモーダル表示、呼び出し側や他のWindowも同時に操作できるようにしたい場合にはモードレス表示で呼び出します。

```csharp
//モーダル表示
dialogService.ShowModalWindow<LicenseView>();
//モードレス表示
dialogService.ShowModelessWindow<LicenseView>();\
```

パワーメータを渡す場合には、引数に指定します。

```csharp
//モーダル表示
dialogService.ShowModalWindow<LicenseView>("test");
//モードレス表示
dialogService.ShowModelessWindow<LicenseView>("test");
```

### 画面の閉じ方

IDialogAwareを継承した時に実装を強制されるRequestCloseを使うことで、表示されているWIndow側から自分自身を閉じることができます。

```csharp
RequestClose?.Invoke(this, true);
```