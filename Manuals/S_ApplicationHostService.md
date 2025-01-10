# ApplicationHostService

アプリを起動したときや、アプリを終了した時の処理を実装、アプリ実行中にバックグラウンドで処理するものを実装する。

標準では、起動時に設定を読み込むコードが実装されている。

```csharp
public class ApplicationHostService : IHostedService
{
    //アプリ起動時に実行されるコード
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Settinghelper.SettingLoad();
        return Task.CompletedTask;
    }

    //アプリ終了時に実行されるコード
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```