using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Template.Domain.Helpers;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;

namespace Template.WPF.ViewModels
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        private readonly MessageService _message;
        private readonly TransitionService _transition;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<SettingGroupEntity> SettingGroups { get; } = new ObservableCollection<SettingGroupEntity>();

        public ReactiveCommand SaveButton { get; }
        public ReactiveCommand ReturnButton { get; }

        public SettingViewModel(MessageService message, TransitionService transition)
        {
            _message = message;
            _transition = transition;

            SettingGroups.Add(CreateGeneralSettings());
            SettingGroups.Add(CreatePasswordSettings());

            SaveButton = new ReactiveCommand().WithSubscribe(SaveButtonExecute);
            ReturnButton = new ReactiveCommand().WithSubscribe(() =>
            {
                transition.NavigateTo<HomeView>();
            });
        }

        private void SaveButtonExecute()
        {
            foreach (var group in SettingGroups)
            {
                foreach (var item in group.SettingItems)
                {
                    if (item is null)
                    {
                        _message.ShowSnackbar($"{item.ViewText}を確認してください");
                        return;
                    }
                    if (item.InputHasErrors())
                    {
                        _message.ShowSnackbar($"{item.ViewText}を確認してください");
                        return;
                    }
                    if (item.ValidateAction() == false)
                    {
                        _message.ShowSnackbar($"{item.ViewText}を確認してください");
                        return;
                    }
                }
            }

            foreach (var group in SettingGroups)
            {
                foreach (var item in group.SettingItems)
                {
                    if(item.SaveAction is null)
                    {
                        continue;
                    }
                    item.SaveAction();
                }
            }
            Settinghelper.SettingSave();

            _message.ShowSnackbar("設定を保存しました");
            _transition.NavigateTo<HomeView>();
        }

        private SettingGroupEntity CreateGeneralSettings()
        {
            var settings = new List<ISettingItem>();
            settings.Add(new SettingItemEntity<string>("設定項目1", "aaaaa")
            .SetSaveAction(entity =>
            {
                var setting = entity.SettingItem.Value;
            })
            .SetStringMinLengthValidation(5));

            return new SettingGroupEntity("一般設定", settings, true);
        }

        private SettingGroupEntity CreatePasswordSettings()
        {
            var IsEnabledpasswordLock = new SettingItemEntity<bool>("パスワードロック", Settinghelper.Instance.IsEnabledSettingPasswordLock, ItemType.ToggleButton)
            .SetSaveAction(entity =>
            {
                if (entity.SettingItem.Value == false)
                {
                    Settinghelper.Instance.IsEnabledSettingPasswordLock = false;
                    return;
                }

                if (Settinghelper.Instance.IsEnabledSettingPasswordLock == true && entity.SettingItem.Value)
                {
                    return;
                }

                var pass = _message.ShowPasswordDialog("設定するパスワードを入力してください");
                if (string.IsNullOrWhiteSpace(pass))
                {
                    Settinghelper.Instance.IsEnabledSettingPasswordLock = false;
                    Settinghelper.Instance.SettingPasswordHash = string.Empty;
                    Settinghelper.Instance.SettingPasswordSalt = string.Empty;
                    return;
                }

                var passCheck = _message.ShowPasswordDialog($"同じパスワードを入力してください{Environment.NewLine}（パスワード再確認）");
                while (pass != passCheck)
                {
                    if (string.IsNullOrWhiteSpace(pass))
                    {
                        Settinghelper.Instance.IsEnabledSettingPasswordLock = false;
                        Settinghelper.Instance.SettingPasswordHash = string.Empty;
                        Settinghelper.Instance.SettingPasswordSalt = string.Empty;
                        return;
                    }
                    _message.ShowMessageDialog("パスワードが一致しません");
                    passCheck = _message.ShowPasswordDialog($"同じパスワードを入力してください{Environment.NewLine}（パスワード再確認）");
                }

                var salt = CipherHelper.GenerateSalt();
                Settinghelper.Instance.SettingPasswordHash = CipherHelper.GetHash(pass, salt);
                Settinghelper.Instance.SettingPasswordSalt = salt;
                Settinghelper.Instance.IsEnabledSettingPasswordLock = entity.SettingItem.Value;
            });

            var settings = new List<ISettingItem>
            {
                IsEnabledpasswordLock,
            };

            return new SettingGroupEntity("パスワード設定", settings, false);
        }
    }
}
