using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Template.Domain.Helpers;
using Template.WPF.Services;
using Template.WPF.Views;

namespace Template.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly TransitionService _navigation;
        private readonly MessageService _message;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactivePropertySlim<string> InputText { get; } = new ReactivePropertySlim<string>();
        public ReactiveCommand LoginButton { get; }
        public ReactiveCommand HaltButton { get; }
        public LoginViewModel(TransitionService navigation, MessageService message)
        {
            _navigation = navigation;
            _message = message;

            LoginButton = new ReactiveCommand().WithSubscribe(LoginButtonExecute); ;
            HaltButton = new ReactiveCommand().WithSubscribe(() =>
            {
                Application.Current.Shutdown();
            });
#if DEBUG
            InputText.Value = "debugger";
#endif
        }

        private void LoginButtonExecute()
        {
            if (string.IsNullOrWhiteSpace(InputText.Value))
            {
                _message.ShowSnackbar("ユーザー名を入力してください");
                return;
            }
            else
            {
                AppDataHelper.Instance.LoginUser = InputText.Value;
                _navigation.NavigateTo<HomeView>();
            }
        }
    }
}
