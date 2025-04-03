using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Template.Infrastructure.SQLite;
using Template.WPF.Services;
using Template.WPF.ViewModels;
using Template.WPF.Views;

namespace Template.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        private Mutex _mutex = null;
        private bool _enableMultipleLaunch = true;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                        .ConfigureServices(ConfigureServices)
                        .Build();
            DispatcherUnhandledException += App_DispatcherUnhandleException;
            if (_enableMultipleLaunch == false)
            {
                _mutex = new Mutex(false, Assembly.GetExecutingAssembly().GetName().Name);
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (!_enableMultipleLaunch && (_mutex is null || !_mutex.WaitOne(0, false)))
            {
                MessageBox.Show("既に起動しています", "", MessageBoxButton.OK);
                Current.Shutdown();
                return;
            }

            // ホストの起動
            await _host.StartAsync();

            base.OnStartup(e);

            // MainWindowの取得と表示
            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            await _host.StopAsync();
            _host.Dispose();
            _mutex?.ReleaseMutex();
            _mutex?.Close();
            _mutex = null;
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

            List<string> message = new List<string>
            {
                "エラーが発生しました",
                $"Error in: {e.Exception.TargetSite.Name}",
                $"Message: {e.Exception.Message}",
                "継続しますか？"
            };

            if (_host.Services.GetRequiredService<MessageService>().ShowErrorDialog("予期しないエラー", string.Join(Environment.NewLine, message)))
            {
                e.Handled = true;
            }
            else
            {
                Current.Shutdown();
            }
        }

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
            services.AddTransient<LabelView>();
            services.AddTransient<LabelViewModel>();
            services.AddTransient<DashboardView>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<GenpinLabelView>();
            services.AddTransient<GenpinLabelViewModel>();
            services.AddTransient<TanabanPrintView>();
            services.AddTransient<TanabanPrintViewModel>();

            //TODO:画面追加対応
        }
    }
}
