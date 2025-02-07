﻿using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
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

        public ReactivePropertySlim<string> Title { get; } = new ReactivePropertySlim<string>("NeoToppas"); //TODO title
        public ReactivePropertySlim<object> ActiveView { get; } = new ReactivePropertySlim<object>();
        public ReactivePropertySlim<bool> IsProgressDialogOpen { get; }
        public ReactivePropertySlim<string> ProgressDialogMessage { get; }
        public ReactivePropertySlim<string> SidebarText { get; } = new ReactivePropertySlim<string>("<");
        public ReactivePropertySlim<double> SidebarWidth { get; } = new ReactivePropertySlim<double>(180);
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

            if (args.Any(a=>a=="License")) //TODO:修正
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
            else
            {
                _navigation.NavigateTo<HomeView>();
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
                SidebarWidth.Value = 180;
            }
        }

        public void TreeSelectedChange(string name)
        {
            switch (name)
            {
                case "PartsSearch":
                    if (ActiveView.Value is not HomeView)
                        _navigation.NavigateTo<HomeView>();
                    break;
                case "License":
                    if (ActiveView.Value is not LicenseView)
                        _navigation.NavigateTo<LicenseView>();
                    break;
                case "PartsLabelPrint":
                    if (ActiveView.Value is not LabelView)
                        _navigation.NavigateTo<LabelView>();
                    break;
            }
        }
    }
}