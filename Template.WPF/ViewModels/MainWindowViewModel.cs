using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System.ComponentModel;
using Template.WPF.Services;
using Template.WPF.Views;

namespace Template.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly TransitionService _navigation;
        private readonly MessageService _message;
        private readonly DialogCoordinator _dialogCoordinator = new DialogCoordinator();
        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactivePropertySlim<string> Title { get; } = new ReactivePropertySlim<string>("NeoToppas"); //TODO:タイトル
        public ReactivePropertySlim<object> ActiveView { get; } = new ReactivePropertySlim<object>();
        public ReactivePropertySlim<bool> IsProgressDialogOpen { get; } 
        public ReactivePropertySlim<string> ProgressDialogMessage { get; }
        public ReactivePropertySlim<SnackbarMessageQueue> SnackbarMessages { get; }

        public ReactiveCommand Loaded { get; }
        public ReactiveCommand ContentRendered { get; }
        public ReactiveCommand Closed { get; }

        public MainWindowViewModel(TransitionService navigation, MessageService messageService)
        {
            // ContentNavigationServiceの初期設定
            _navigation = navigation;
            _navigation.Initialize(view =>
            {
                ActiveView.Value = view;
            });

            // ContentMessageServiceの初期設定
            _message = messageService;
            _message.SetDialogContext(this);
            ProgressDialogMessage = _message.ProgressDialogMessage;
            IsProgressDialogOpen = _message.IsProgressDialogOpen;
            SnackbarMessages = _message.SnackbarMessages;

            Loaded = new ReactiveCommand().WithSubscribe(() =>
            {
                // 呼び出された後の処理
            });
            ContentRendered = new ReactiveCommand().WithSubscribe(() =>
            {
                // 画面が表示された後の処理
            });
            Closed = new ReactiveCommand().WithSubscribe(() =>
            {
                // 画面が閉じられた後の処理
            });

            // 初期画面の表示
            _navigation.NavigateTo<HomeView>();
        }
    }
}