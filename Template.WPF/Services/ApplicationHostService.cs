using Microsoft.Extensions.Hosting;
using Template.Domain.Helpers;
using Template.Infrastructure.SQLite;

namespace Template.WPF.Services
{
    public class ApplicationHostService : IHostedService
    {
        //アプリ起動時に実行されるコード
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Settinghelper.SettingLoad();
            SQLiteFileManager.CreateDatabaseFile();
            return Task.CompletedTask;
        }

        //アプリ終了時に実行されるコード
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
