using NeoToppas.WPF.Services;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Template.Domain.Entities;
using Template.Domain.Helpers;
using Template.Infrastructure.SQLite;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;
using System.Windows.Shapes;
using System.Data;
using Npgsql;
using System.Transactions;
using System.Diagnostics;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using NeoToppas.Infrastructure.Postgres;

namespace Template.WPF.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly TransitionService _navigation;
        private readonly MessageService _message;
        private readonly DialogService _dialog;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactivePropertySlim<string> AppText { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> VersionNo { get; } = new ReactivePropertySlim<string>("Ver " + Assembly.GetExecutingAssembly().GetName().Version);
        public ReactivePropertySlim<DataTable> TradingCompanyTbl { get; set; } = new ReactivePropertySlim<DataTable>(new DataTable());
        public ReactivePropertySlim<string> param1Input { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> param2Input { get; } = new ReactivePropertySlim<string>();
        public ReactiveCommand SettingMenuButton { get; }
        public ReactiveCommand InformationButton { get; }
        public ReactiveCommand ApplicationHaltButton { get; }
        public ReactiveCommand LogoutButton { get; }
        public ReactiveCommand DBSelect1Button { get; }
        public ReactiveCommand DBInsert1Button { get; }
        public ReactiveCommand DBUpdate1Button { get; }
        public ReactiveCommand DBDelete1Button { get; }

        public ObservableCollection<MenuButtonEntity> MenuButtons { get; } = new ObservableCollection<MenuButtonEntity>();
        /// <summary>
		/// コマンドライン引数
		/// </summary>
		private readonly List<string> args = new(Environment.GetCommandLineArgs());


        public HomeViewModel(TransitionService navigation, MessageService message, DialogService dialogService)
        {
            _navigation = navigation;
            _message = message;
            _dialog = dialogService;
            param1Input.Value = "";
            param2Input.Value = "";
            // アプリケーション名の設定
            AppText.Value = GetApplicationName();

            SettingMenuButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                if (Settinghelper.Instance.IsEnabledSettingPasswordLock == false || Settinghelper.Instance.SettingPasswordHash == string.Empty)
                {
                    _navigation.NavigateTo<SettingView>();
                    return;
                }

                var ret = _message.ShowPasswordDialog("パスワードを入力してください");
                if (ret == string.Empty)
                {
                    return;
                }
                if (CipherHelper.GetHash(ret, Settinghelper.Instance.SettingPasswordSalt) == Settinghelper.Instance.SettingPasswordHash)
                {
                    _navigation.NavigateTo<SettingView>();
                }
                else
                {
                    _message.ShowSnackbar("パスワードが違います");
                }
            });

            InformationButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                _navigation.NavigateTo<LicenseView>();
            });

            LogoutButton = new ReactiveCommand().WithSubscribe(() =>
            {
                if (_message.ShowYesCancelDialog("現在のユーザーからログアウトしますか？？"))
                {
                    AppDataHelper.Instance.LoginUser = string.Empty;
                    _navigation.NavigateTo<LoginView>();
                }
            });

            ApplicationHaltButton = new ReactiveCommand().WithSubscribe(() =>
            {
                if (_message.ShowYesCancelDialog("終了しますか？"))
                {
                    Application.Current.Shutdown();
                }
            });

            DBSelect1Button = new ReactiveCommand().WithSubscribe(() =>
            {
                try 
                {
                    string cmd_str = "SELECT * FROM public.\"TradingCompany\" ORDER BY \"TradingCompanyCode\" ASC";
                    TradingCompanyTbl.Value = PostgresBase.GetDataTable(cmd_str);
                    //SELECT結果表示
                    for (int i = 0; i < TradingCompanyTbl.Value.Rows.Count; i++)
                    {
                        Debug.WriteLine("(col1, col2) = (" + TradingCompanyTbl.Value.Rows[i][0] + ", " + TradingCompanyTbl.Value.Rows[i][1] + ")");
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"データベースエラー1: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            DBInsert1Button = new ReactiveCommand().WithSubscribe(() =>
            {
                try
                {
                    string cmd_str = String.Concat("Insert into public.\"TradingCompany\"(\"TradingCompanyCode\",\"TradingCompanyName\") Values(",
                        param1Input,",'",param2Input,"')");
                    int result = PostgresBase.InsertDataTable(cmd_str);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"データベースエラー2: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            //自動アップデート確認
            if (CommonConst.ENABLE_AUTOUPDATE)
            {
                Update();
            }
        }

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


        private void Update()
        {
            if (args.Any(a => a == "/up") == false)
            {
                // 今回は実験なので拡張子がtxtのだけ更新
                // Todo:アップデート機能要監視 
                ApplicationUpdate update = new();

                if (!Directory.Exists(CommonConst.DATA_FOLDER))
                {
                    MessageBox.Show("自動アップデート失敗！\r\n最新アプリの格納フォルダにアクセスできません！！", "エラー",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                else 
                {
                    if (!File.Exists(Directory.GetParent(CommonConst.DATA_FOLDER).ToString() + @"\NeoToppasLatestVersionTXT.txt"))
                    {
                        MessageBox.Show("自動アップデート失敗！\r\n最新バージョン情報ファイルにアクセスできません！！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                    {
                        StreamReader sr = new StreamReader(Directory.GetParent(CommonConst.DATA_FOLDER).ToString() + @"\NeoToppasLatestVersionTXT.txt");

                        string str;
                        str = sr.ReadLine();
                        sr.Close();

                        if (str == CommonConst.APP_VERSION)
                        {
                            //更新なし
                           
                        }
                        else
                        {
                            if (update.Update(CommonConst.DATA_FOLDER, null) == true) //データ格納場所
                            {
                                //更新なし
                            }
                        }
                    }
                }

            }
            else
            {
                // コマンドライン引数に「/up」があれば更新処理があったので拡張子が「delete」の古いファイルを取得し削除
                foreach (var f in Directory.GetFiles(Environment.CurrentDirectory, "*.delete"))
                {
                    File.Delete(f);
                }
            }
        }
    }
}
