using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using Template.WPF.Services;
using Template.WPF.Views;
using Toppas4.Services;

namespace Template.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly TransitionService _navigation;
        private readonly MessageService _message;
        private readonly DialogCoordinator _dialogCoordinator = new DialogCoordinator();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactivePropertySlim<string> Title { get; } = new ReactivePropertySlim<string>("Toppas4"); //TODO title
        public ReactivePropertySlim<object> ActiveView { get; } = new ReactivePropertySlim<object>();
        public ReactivePropertySlim<int> TreeSize { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> TreeFont1 { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> TreeFont2 { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<bool> IsProgressDialogOpen { get; }
        public ReactivePropertySlim<string> ProgressDialogMessage { get; }
        public ReactivePropertySlim<string> SidebarText { get; } = new ReactivePropertySlim<string>("<");
        public ReactivePropertySlim<double> SidebarWidth { get; } = new ReactivePropertySlim<double>(230);
        public ReactivePropertySlim<SnackbarMessageQueue> SnackbarMessages { get; }

        public ReactiveCommand Loaded { get; }
        public ReactiveCommand ContentRendered { get; }
        public ReactiveCommand Closed { get; }
        public ReactiveCommand SidebarTextMouseUp { get; }
        private readonly List<string> args = new(Environment.GetCommandLineArgs());

        public MainWindowViewModel(TransitionService navigation, MessageService messageService)
        {
            // TransitionServiceの初期化
            _navigation = navigation;
            _navigation.Initialize(view =>
            {
                ActiveView.Value = view;
                TreeSize.Value = 15;
                TreeFont1.Value = 20;
                TreeFont2.Value = 15;   
            });

            // MessageServiceの初期化
            _message = messageService;
            _message.SetDialogContext(this);
            ProgressDialogMessage = _message.ProgressDialogMessage;
            IsProgressDialogOpen = _message.IsProgressDialogOpen;
            SnackbarMessages = _message.SnackbarMessages;

            // コマンド初期化
            Loaded = new ReactiveCommand().WithSubscribe(OnLoaded);
            ContentRendered = new ReactiveCommand().WithSubscribe(OnContentRendered);
            Closed = new ReactiveCommand().WithSubscribe(OnClosed);
            SidebarTextMouseUp = new ReactiveCommand().WithSubscribe(OnSidebarTextMouseUp);

            if (args.Any(a=>a=="License")) //TODO:画面追加対応
            { 
                _navigation.NavigateTo<LicenseView>(); 
            }
            else if (args.Any(a => a == "PartsSearch")) 
            {
                _navigation.NavigateTo<HomeView>();
            }
            else if (args.Any(a => a == "PartsLabelPrint"))
            {
                _navigation.NavigateTo<LabelView>();
            }
            else if (args.Any(a => a == "Dashboard"))
            {
                _navigation.NavigateTo<DashboardView>();
            }
            else if (args.Any(a => a == "GenpinLabel"))
            {
                _navigation.NavigateTo<GenpinLabelView>();
            }
            else if (args.Any(a => a == "TanabanPrint"))
            {
                _navigation.NavigateTo<TanabanPrintView>();
            }
            else if (args.Any(a => a == "TanafudaPrint"))
            {
                _navigation.NavigateTo<TanafudaPrintView>();
            }
            else if (args.Any(a => a == "KibanLabel"))
            {
                _navigation.NavigateTo<KibanLabelView>();
            }
            else if (args.Any(a => a == "EtcLabel"))
            {
                _navigation.NavigateTo<EtcLabelView>();
            }
            else if (args.Any(a => a == "BomConverter"))
            {
                _navigation.NavigateTo<BomConverterView>();
            }
            else if (args.Any(a => a == "ShippingList"))
            {
                _navigation.NavigateTo<ShippingListView>();
            }
            else
            {
                _navigation.NavigateTo<DashboardView>();
            }

            if (Toppas4.Properties.Settings.Default.AUTOUPDATE_ENABLE) //CommonConst.AUTOUPDATE_ENABLE
            {
                Update(); //プログラムの自動アップデート
            }
        }

        private void OnLoaded()
        {
            // ロード時の処理をここに記述
        }

        private void OnContentRendered()
        {
            // 表示後の処理をここに記述
        }

        private void OnClosed()
        {
            // クローズ時の処理をここに記述
        }

        private void Update()
        {
            if (args.Any(a => a == "/up") == false)
            {
                ApplicationUpdate update = new();

                if (!Directory.Exists(Toppas4.Properties.Settings.Default.AUTOUPDATE_DATA_FOLDER)) //CommonConst.AUTOUPDATE_DATA_FOLDER
                {
                    MessageBox.Show("自動アップデート失敗！\r\n最新アプリの格納フォルダにアクセスできません！！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (!File.Exists((Toppas4.Properties.Settings.Default.AUTOUPDATE_DATA_FOLDER).ToString() + @"\" + CommonConst.AUTOUPDATE_VERSION_FILE)) //if (!File.Exists(Directory.GetParent(CommonConst.AUTOUPDATE_DATA_FOLDER).ToString() + @"\NeoToppasLatestVersionTXT.txt"))
                    {
                        MessageBox.Show("自動アップデート失敗！\r\n最新バージョン情報ファイルにアクセスできません！！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        //ファイルサーバ上のバージョン確認
                        StreamReader sr = new StreamReader(Toppas4.Properties.Settings.Default.AUTOUPDATE_DATA_FOLDER + @"\" + CommonConst.AUTOUPDATE_VERSION_FILE);
                        string str;
                        str = sr.ReadLine();
                        sr.Close();
                        //現在の実行ファイルののバージョン確認
                        StreamReader sr2 = new StreamReader(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\" + CommonConst.AUTOUPDATE_VERSION_FILE);
                        //string str2;
                        CommonConst.AppVersion = sr2.ReadLine();
                        sr2.Close();

                        if (str == CommonConst.AppVersion)
                        {
                        }
                        else
                        {
                            if (update.Update(Toppas4.Properties.Settings.Default.AUTOUPDATE_DATA_FOLDER, null) == true)
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


        private void OnSidebarTextMouseUp()
        {
            if (SidebarText.Value == "<")
            {
                SidebarText.Value = ">";
                SidebarWidth.Value = 0;
            }
            else
            {
                SidebarText.Value = "<";
                SidebarWidth.Value = 230;
            }
        }

        public void TreeSelectedChange(string name) //TODO:ページ追加対応
        {
            switch (name)
            {
                case "PartsSearch":
                    if (ActiveView.Value is not HomeView)
                        _navigation.NavigateTo<HomeView>();
                    break;
                case "PartsQuantityChk_Prototypee":
                    if (ActiveView.Value is not LicenseView)
                        _navigation.NavigateTo<LicenseView>();
                    break;
                case "PartsQuantityChk_MassPro":
                    if (ActiveView.Value is not HomeView)
                        _navigation.NavigateTo<HomeView>();
                    break;
                case "PartsChangeCtrl":
                    if (ActiveView.Value is not LicenseView)
                        _navigation.NavigateTo<LicenseView>();
                    break;
                case "PartsLabelPrint":
                    if (ActiveView.Value is not LabelView)
                        _navigation.NavigateTo<LabelView>();
                    break;
                case "BoardSheetLabelPrint":
                    if (ActiveView.Value is not HomeView)
                        _navigation.NavigateTo<HomeView>();
                    break;
                case "BoardPieceLabelPrint":
                    if (ActiveView.Value is not LicenseView)
                        _navigation.NavigateTo<LicenseView>();
                    break;
                //case "LabelGenpinPrint":
                //    if (ActiveView.Value is not LabelGenpinView)
                //        _navigation.NavigateTo<LabelGenpinView>();
                //    break;
                case "Dashboard":
                    if (ActiveView.Value is not DashboardView)
                        _navigation.NavigateTo<DashboardView>();
                    break;
                case "GenpinLabel":
                    if (ActiveView.Value is not GenpinLabelView)
                        _navigation.NavigateTo<GenpinLabelView>();
                    break;
                case "TanabanPrint":
                    if (ActiveView.Value is not TanabanPrintView)
                        _navigation.NavigateTo<TanabanPrintView>();
                    break;
                case "TanafudaPrint":
                    if (ActiveView.Value is not TanafudaPrintView)
                        _navigation.NavigateTo<TanafudaPrintView>();
                    break;
                case "KibanLabel":
                    if (ActiveView.Value is not KibanLabelView)
                        _navigation.NavigateTo<KibanLabelView>();
                    break;
                case "EtcLabel":
                    if (ActiveView.Value is not EtcLabelView)
                        _navigation.NavigateTo<EtcLabelView>();
                    break;
                case "BomConverter":
                    if (ActiveView.Value is not BomConverterView)
                        _navigation.NavigateTo<BomConverterView>();
                    break;
                case "ShippingList":
                    if (ActiveView.Value is not ShippingListView)
                        _navigation.NavigateTo<ShippingListView>();
                    break;




            }
        }
    }
}