using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System.Windows;
using Template.WPF.ViewModels;

namespace Template.WPF.Services
{
    public class MessageService
    {
        private readonly DialogCoordinator _dialogCoordinator = new DialogCoordinator();
        private object _context;

        public ReactivePropertySlim<SnackbarMessageQueue> SnackbarMessages { get; } = new ReactivePropertySlim<SnackbarMessageQueue>(new SnackbarMessageQueue());
        public ReactivePropertySlim<string> ProgressDialogMessage { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> IsProgressDialogOpen { get; } = new ReactivePropertySlim<bool>();

        public MessageService()
        {
        }

        public void SetDialogContext(object context)
        {
            _context = context;
        }

        // Snackbarのメソッド
        public void ShowSnackbar(string message)
        {
            SnackbarMessages.Value.Enqueue(message);
        }

        // ダイアログのメソッド
        public void ShowMessageDialog(string message)
        {
            ShowMessageDialog(message, "OK");
        }

        public void ShowMessageDialog(string message, string buttonText)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = buttonText,
                DialogMessageFontSize = 24,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            ShowModalMessageExternal(message, MessageDialogStyle.Affirmative, metroDialogSettings);
        }

        public void ShowCautionDialog(string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                DialogMessageFontSize = 32,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Accented,
            };

            ShowModalMessageExternal(message, MessageDialogStyle.Affirmative, metroDialogSettings);
        }

        public bool ShowErrorDialog(string title, string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "はい",
                NegativeButtonText = "いいえ",
                DialogTitleFontSize = 32,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Accented,
            };

            return ShowModalMessageExternal(title, message, MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings) == MessageDialogResult.Affirmative;
        }

        public bool ShowYesNoDialog(string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "はい",
                NegativeButtonText = "いいえ",
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            return ShowModalMessageExternal(message, MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings) == MessageDialogResult.Affirmative;
        }

        public bool ShowYesCancelDialog(string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "はい",
                NegativeButtonText = "キャンセル",
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            return ShowModalMessageExternal(message, MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings) == MessageDialogResult.Affirmative;
        }

        public string ShowInputDialog(string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                DialogMessageFontSize = 32,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            if (Application.Current.Dispatcher.CheckAccess())
            {
                return _dialogCoordinator.ShowModalInputExternal(_context, string.Empty, message, metroDialogSettings);
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() => _dialogCoordinator.ShowModalInputExternal(_context, string.Empty, message, metroDialogSettings));
            }
        }

        public string ShowPasswordDialog(string message)
        {
            var metroDialogSettings = new LoginDialogSettings()
            {
                AffirmativeButtonText = "OK",
                DialogMessageFontSize = 32,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
                ShouldHideUsername = true,
            };

            if (Application.Current.Dispatcher.CheckAccess())
            {
                return _dialogCoordinator.ShowModalLoginExternal(_context, string.Empty, message, metroDialogSettings).Password;
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() => _dialogCoordinator.ShowModalLoginExternal(_context, string.Empty, message, metroDialogSettings).Password);
            }
        }

        public (string user, string password) ShowLoginDialog(string message)
        {
            var metroDialogSettings = new LoginDialogSettings()
            {
                AffirmativeButtonText = "OK",
                DialogMessageFontSize = 32,
                AnimateHide = true,
                AnimateShow = false,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            if (Application.Current.Dispatcher.CheckAccess())
            {
                var result = _dialogCoordinator.ShowModalLoginExternal(_context, string.Empty, message, metroDialogSettings);
                return (result.Username, result.Password);
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    var result = _dialogCoordinator.ShowModalLoginExternal(_context, string.Empty, message, metroDialogSettings);
                    return (result.Username, result.Password);
                });
            }
        }

        private MessageDialogResult ShowModalMessageExternal(string message, MessageDialogStyle style, MetroDialogSettings setting)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                return _dialogCoordinator.ShowModalMessageExternal(_context, string.Empty, message, style, setting);
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() =>
                    _dialogCoordinator.ShowModalMessageExternal(_context, string.Empty, message, style, setting));
            }
        }

        private MessageDialogResult ShowModalMessageExternal(string title, string message, MessageDialogStyle style, MetroDialogSettings setting)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                return _dialogCoordinator.ShowModalMessageExternal(_context, title, message, style, setting);
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() =>
                    _dialogCoordinator.ShowModalMessageExternal(_context, title, message, style, setting));
            }
        }

        // 処理中ダイアログのメソッド
        public async Task ShowProcessingDialogAsync(string message, Action action)
        {
            ProgressDialogMessage.Value = message;
            IsProgressDialogOpen.Value = true;
            await Task.Run(action);
            IsProgressDialogOpen.Value = false;
        }

        public async Task ShowProcessingDialogBeforeAsync(string message, Action action, Action beforeAction)
        {
            beforeAction.Invoke();
            ProgressDialogMessage.Value = message;
            IsProgressDialogOpen.Value = true;
            await Task.Run(action);
            IsProgressDialogOpen.Value = false;
        }

        public async Task ShowProcessingDialogAfterAsync(string message, Action action, Action afterAction)
        {
            ProgressDialogMessage.Value = message;
            IsProgressDialogOpen.Value = true;
            await Task.Run(action);
            IsProgressDialogOpen.Value = false;
            afterAction.Invoke();
        }
    }
}
