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
using NeoToppas.Domain.Entities;

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
        public ObservableCollection<TradingCompanyEntities> TradingCompanyTbl { get; set; } = new ObservableCollection<TradingCompanyEntities>();
        public ReactivePropertySlim<string> param1Input { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> param2Input { get; } = new ReactivePropertySlim<string>();
        private TradingCompanyEntities _selectedTradingCompany;
        public TradingCompanyEntities SelectedTradingCompany
        {
            get => _selectedTradingCompany;
            set
            {
                if (_selectedTradingCompany != value)
                {
                    _selectedTradingCompany = value;
                    OnPropertyChanged(nameof(SelectedTradingCompany));
                    OnSelectedTradingCompanyChanged();
                }
            }
        }
        public ReactiveCommand SettingMenuButton { get; }
        public ReactiveCommand InformationButton { get; }
        public ReactiveCommand ApplicationHaltButton { get; }
        public ReactiveCommand LogoutButton { get; }
        public ReactiveCommand DBSelect1Button { get; }
        public ReactiveCommand DBInsert1Button { get; }
        public ReactiveCommand DBUpdate1Button { get; }
        public ReactiveCommand DBDelete1Button { get; }
        public ReactiveCommand CompanyGridSelectionChanged { get; }

        public ObservableCollection<MenuButtonEntity> MenuButtons { get; } = new ObservableCollection<MenuButtonEntity>();
        private readonly List<string> args = new(Environment.GetCommandLineArgs());

        public HomeViewModel(TransitionService navigation, MessageService message, DialogService dialogService)
        {
            _navigation = navigation;
            _message = message;
            _dialog = dialogService;
            param1Input.Value = "";
            param2Input.Value = "";
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
                DBSelect1_Exe();
            });

            DBInsert1Button = new ReactiveCommand().WithSubscribe(() =>
            {
                try
                {
                    string cmd_str = String.Concat("Insert into public.\"TradingCompany\"(\"TradingCompanyCode\",\"TradingCompanyName\") Values(",
                        param1Input, ",'", param2Input, "')");
                    int result = PostgresBase.InsertDataTable(cmd_str);
                    DBSelect1_Exe();
                    if (result == 1) _message.ShowSnackbar("登録成功");
                    else _message.ShowErrorDialog("エラー", "登録失敗");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"データベースエラー2: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            DBUpdate1Button = new ReactiveCommand().WithSubscribe(() =>
            {
                try
                {
                    string cmd_str = String.Concat("update public.\"TradingCompany\" Set \"TradingCompanyName\" = '",param2Input,"' where \"TradingCompanyCode\" ='",param1Input,"'");
                    int result = PostgresBase.UpdateDataTable(cmd_str);
                    DBSelect1_Exe();
                    if (result == 1) _message.ShowSnackbar("登録成功");
                    else _message.ShowErrorDialog("エラー", "登録失敗");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"データベースエラー2: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            if (CommonConst.ENABLE_AUTOUPDATE)
            {
                Update(); //プログラムの自動アップデート
            }
            DBSelect1_Exe();
        }

        private void OnSelectedTradingCompanyChanged()
        {
            if (SelectedTradingCompany != null)
            {
                //MessageBox.Show($"選択された会社: {SelectedTradingCompany.TradingCompanyName}", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                param1Input.Value = SelectedTradingCompany.TradingCompanyCode.ToString();
                param2Input.Value = SelectedTradingCompany.TradingCompanyName;

            }
        }

        private void DBSelect1_Exe()
        {
            try
            {
                string cmd_str = "SELECT * FROM public.\"TradingCompany\" ORDER BY \"TradingCompanyCode\" ASC";
                var dataTable = PostgresBase.GetDataTable(cmd_str);
                TradingCompanyTbl.Clear();
                foreach (DataRow row in dataTable.Rows)
                {
                    TradingCompanyTbl.Add(new TradingCompanyEntities
                    {
                        TradingCompanyCode = Convert.ToInt32(row["TradingCompanyCode"]),
                        TradingCompanyName = row["TradingCompanyName"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データベースエラー1: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ApplicationUpdate update = new();

                if (!Directory.Exists(CommonConst.DATA_FOLDER))
                {
                    MessageBox.Show("自動アップデート失敗！\r\n最新アプリの格納フォルダにアクセスできません！！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        }
                        else
                        {
                            if (update.Update(CommonConst.DATA_FOLDER, null) == true)
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var f in Directory.GetFiles(Environment.CurrentDirectory, "*.delete"))
                {
                    File.Delete(f);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}