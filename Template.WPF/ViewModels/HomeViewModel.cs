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
                string conn_str = "Server=10.192.139.9; Port=5432; User Id=konishi; Password=konishi; Database=NeoToppas;Enlist=true";
                try 
                { 
                    //TransactionScopeの利用
                    using (TransactionScope ts = new TransactionScope())
                    {
                        using (NpgsqlConnection conn2 = new NpgsqlConnection(conn_str))
                        {
                            NpgsqlCommand cmd = null;
                            string cmd_str = null;
                            NpgsqlDataAdapter da = null;

                            //PostgreSQLへ接続後、SELECT結果を取得
                            conn2.Open();

                            //SELECT処理
                            cmd_str = "SELECT * FROM public.\"TradingCompany\" ORDER BY \"TradingCompanyCode\" ASC"; 
                            cmd = new NpgsqlCommand(cmd_str, conn2);
                            da = new NpgsqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            TradingCompanyTbl.Value = dt;

                            //SELECT結果表示
                            for (int i = 0; i < TradingCompanyTbl.Value.Rows.Count; i++)
                            {
                                Debug.WriteLine("(col1, col2) = (" + TradingCompanyTbl.Value.Rows[i][0] + ", " + TradingCompanyTbl.Value.Rows[i][1] + ")");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"データベースエラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
