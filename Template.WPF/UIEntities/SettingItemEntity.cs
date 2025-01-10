using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System.Text.RegularExpressions;
using System.Windows;
using Template.WPF.Helpers;

namespace Template.WPF.UIEntities
{
    public enum ItemType
    {
        TextBox,
        ToggleButton,
        ComboBox,
        DirectorySelect,
        FileSelect,
        ButtonOnly,
    }
    public class SettingItemEntity<T> : ISettingItem, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ReactiveProperty<T> SettingItem { get; }
        public string ViewText { get; }
        public ReactivePropertySlim<string> ToolTipText { get; }
        public ReactivePropertySlim<bool> IsEnabled { get; private set; } = new ReactivePropertySlim<bool>(true);
        public Func<bool> ValidateAction { get; private set; } = () => { return true; };
        public Action SaveAction { get; private set; }
        public ReactivePropertySlim<Visibility> TextInputAreaVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactivePropertySlim<Visibility> ToggleButtonVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactivePropertySlim<Visibility> ItemsSelectVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactivePropertySlim<Visibility> PathSelectVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactivePropertySlim<Visibility> ActionButtonVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactiveCollection<T> Items { get; } = new ReactiveCollection<T>();

        public ReactiveCommand ButtonCommand { get; }


        public SettingItemEntity(string viewText, T initialValue, ItemType type = ItemType.TextBox, string toolTipText = null)
        {
            SettingItem = new ReactiveProperty<T>(initialValue, ReactivePropertyMode.IgnoreInitialValidationError)
                .SetValidateAttribute(() => SettingItem);
            ViewText = viewText;
            ToolTipText = new ReactivePropertySlim<string>(toolTipText);

            // Set visibility based on type
            switch (type)
            {
                case ItemType.TextBox:
                    TextInputAreaVisibility.Value = Visibility.Visible;
                    break;
                case ItemType.ToggleButton:
                    ToggleButtonVisibility.Value = Visibility.Visible;
                    break;
                case ItemType.ComboBox:
                    ItemsSelectVisibility.Value = Visibility.Visible;
                    break;
                case ItemType.ButtonOnly:
                    ActionButtonVisibility.Value = Visibility.Visible;
                    break;
                case ItemType.DirectorySelect:
                case ItemType.FileSelect:
                    PathSelectVisibility.Value = Visibility.Visible;
                    break;
            }

            // Set ButtonCommand based on type
            ButtonCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                if (type == ItemType.DirectorySelect)
                {
                    SettingItem.Value = (T)Convert.ChangeType(DialogHelper.SelectFolderDialog(), typeof(T));
                }
                else if (type == ItemType.FileSelect)
                {
                    SettingItem.Value = (T)Convert.ChangeType(DialogHelper.SelectFileDialog(), typeof(T));
                }
            }).AddTo(_disposables);
        }

        public SettingItemEntity<T> SetValidateAction(Func<SettingItemEntity<T>, bool> func)
        {
            ValidateAction = () => func(this);
            return this;
        }

        public SettingItemEntity<T> SetSaveAction(Action<SettingItemEntity<T>> act)
        {
            SaveAction = () => act(this);
            return this;
        }

        public SettingItemEntity<T> SetSubscribe(Action<T> act)
        {
            SettingItem.Subscribe(act).AddTo(_disposables);
            return this;
        }

        public SettingItemEntity<T> SetIsEnabled(bool value)
        {
            IsEnabled.Value = value;
            return this;
        }

        public SettingItemEntity<T> SetButtonAction(Action act)
        {
            ButtonCommand.Subscribe(act).AddTo(_disposables);
            return this;
        }

        public SettingItemEntity(string viewText, T initialValue, ItemType type, ReactiveCollection<T> ItemList, string toolTipText = "") : this(viewText, initialValue, type, toolTipText)
        {
            foreach (var item in ItemList)
            {
                Items.Add(item);
            }
        }

        public bool InputHasErrors()
        {
            return SettingItem.HasErrors;
        }

        public SettingItemEntity<T> SetStringRequiredValidation(string errorMessage = "入力は必須です")
        {
            SettingItem.SetValidateNotifyError(value => IsNullOrWhiteSpace(value) ? errorMessage : null);
            SetValidateAction(entity => !IsNullOrWhiteSpace(entity.SettingItem.Value));
            return this;
        }

        public SettingItemEntity<T> SetStringMinLengthValidation(int MinLength, string errorMessage = "")
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                errorMessage = $"{MinLength}文字以上で設定してください";
            }
            SettingItem.SetValidateNotifyError(value => value != null && value.ToString().Length < MinLength ? errorMessage : null);
            SetValidateAction(entity => entity.SettingItem.Value != null && entity.SettingItem.Value.ToString().Length >= MinLength);
            return this;
        }

        public SettingItemEntity<T> SetAlphanumericOrSymbolValidation(string errorMessage = "半角英数字記号のみ使用できます")
        {
            SettingItem.SetValidateNotifyError(value => value != null && !Regex.IsMatch(value.ToString(), @"[a-zA-Z0-9 -/:-@\[-\\{-\~]+") ? errorMessage : null);
            SetValidateAction(entity => entity.SettingItem.Value != null && Regex.IsMatch(entity.SettingItem.Value.ToString(), @"[a-zA-Z0-9 -/:-@\[-\\{-\~]+"));
            return this;
        }

        public SettingItemEntity<T> SetNumericalRangeValidation(double MinValue, double MaxValue, string errorMessage = "")
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                errorMessage = $"{MinValue}以上{MaxValue}以下で設定してください";
            }
            SettingItem.SetValidateNotifyError(value => !IsValidDoubleInRange(value, MinValue, MaxValue) ? errorMessage : null);
            SetValidateAction(entity => IsValidDoubleInRange(entity.SettingItem.Value, MinValue, MaxValue));
            return this;
        }

        private static bool IsNullOrWhiteSpace(object value)
        {
            return value is null || string.IsNullOrWhiteSpace(value.ToString());
        }

        private static bool IsValidDoubleInRange(object value, double min, double max)
        {
            return double.TryParse(value?.ToString(), out double v) && v >= min && v <= max;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }

}