using Reactive.Bindings;
using System.Collections.ObjectModel;

namespace Template.WPF.UIEntities
{
    public sealed class SettingGroupEntity
    {
        public ReactivePropertySlim<string> GroupName { get; }
        public ReactivePropertySlim<bool> IsExpanded { get; }
        public ObservableCollection<ISettingItem> SettingItems { get; }

        public SettingGroupEntity(string groupName, List<ISettingItem> settingItems, bool isExpanded)
        {
            GroupName = new ReactivePropertySlim<string>(groupName);
            IsExpanded = new ReactivePropertySlim<bool>(isExpanded);
            SettingItems = new ObservableCollection<ISettingItem>(settingItems);
        }
        public SettingGroupEntity(string groupName, List<ISettingItem> settingItems) : this(groupName, settingItems, false)
        {
        }
    }
}
