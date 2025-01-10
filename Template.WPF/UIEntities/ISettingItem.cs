using Reactive.Bindings;

namespace Template.WPF.UIEntities
{
    public interface ISettingItem
    {
        string ViewText { get; }
        Func<bool> ValidateAction { get; }
        Action SaveAction { get; }
        ReactivePropertySlim<string> ToolTipText { get; }

        bool InputHasErrors();
    }
}
