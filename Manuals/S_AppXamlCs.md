# App.xaml.cs

App.xaml.csにはいくつかの機能を持たせています。基本的な機能は設定済みであるため、通常使用する際にApp.xaml.csの修正は不要です。

※二重起動の管理のみ

- 未処理例外の受け取り
- 二重起動管理
- DI管理

## 未処理例外の受け取り

想定していない例外が発生した場合、App_DispatcherUnhandleExceptionメソッドに例外が 流れてきます。その際に、ログ出力、ユーザーへの表示、継続確認を行います。

![image.png](image%202.png)

```csharp
public App()
{
    DispatcherUnhandledException += App_DispatcherUnhandleException;
}

private void App_DispatcherUnhandleException(object sender, DispatcherUnhandledExceptionEventArgs e)
{
    string logFilePath = "error_log.txt";
    string errorMessage = $"[{DateTime.Now}] Error in: {e.Exception.TargetSite.Name}" + Environment.NewLine +
                          $"Message: {e.Exception.Message}" + Environment.NewLine +
                          $"Stack Trace: {e.Exception.StackTrace}" + Environment.NewLine;
    try
    {
        File.AppendAllText(logFilePath, errorMessage);
    }
    catch (Exception logException)
    {
        Debug.WriteLine("Error writing to log file: " + logException.Message);
    }

    string userMessage = "予期しないエラーが発生しました";
    MessageBox.Show(userMessage, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);

    e.Handled = true;

    Application.Current.Shutdown();
}
```

## 二重起動管理

_enableMultipleLaunch の変数によって、二重起動を禁止します。

二重起動させたくない場合は_enableMultipleLaunchをfalseに、二重起動を許可する場合はtrueに設定します。

```csharp
private Mutex _mutex = null;
private bool _enableMultipleLaunch = false;
```

```csharp
public App()
{
    if (_enableMultipleLaunch == false)
    {
        _mutex = new Mutex(false, Assembly.GetExecutingAssembly().GetName().Name);
    }
}
```

```csharp
protected override async void OnStartup(StartupEventArgs e)
{
    if (_enableMultipleLaunch || _mutex.WaitOne(0, false) == false)
    {
        MessageBox.Show("既に起動しています", "", MessageBoxButton.OK);
        if (_mutex is not null)
        {
            _mutex.ReleaseMutex();
            _mutex.Close();
            _mutex = null;
        }
        Current.Shutdown();
    }
}
```

```csharp
protected override async void OnExit(ExitEventArgs e)
{
    if (_mutex is not null)
    {
        _mutex.ReleaseMutex();
        _mutex.Close();
        _mutex = null;
    }
}
```

## DI管理

Microsoft.Extensions.Hostingの機能を使い、DIを実装しています。

Serviceや画面を新たに作成した場合はここで登録します。

```csharp
private IHost _host;
```

```csharp
public App()
{
    _host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();
}
```

```csharp
private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    //HostedService登録
    services.AddHostedService<ApplicationHostService>();

    //Service登録
    services.AddSingleton<TransitionService>();
    services.AddSingleton<MessageService>();
    services.AddSingleton<DialogService>();

    //SQLite登録
    services.AddSingleton<UserSQLite>();

    //View / ViewModel登録
    //AddSingleton: 一度生成されたインスタンスを使いまわす
    //AddTransient: 使用するたびに新しいインスタンスを生成する
    services.AddTransient<MainWindow>();
    services.AddTransient<MainWindowViewModel>();
    services.AddTransient<HomeView>();
    services.AddTransient<HomeViewModel>();
    services.AddTransient<LoginView>();
    services.AddTransient<LoginViewModel>();
    services.AddTransient<LicenseView>();
    services.AddTransient<LicenseViewModel>();
    services.AddTransient<SettingView>();
    services.AddTransient<SettingViewModel>();
}
```

```csharp
protected override async void OnStartup(StartupEventArgs e)
{
    // ホストの起動
    await _host.StartAsync();

    // MainWindowの取得と表示
    _host.Services.GetRequiredService<MainWindow>().Show();

    base.OnStartup(e);
}
```