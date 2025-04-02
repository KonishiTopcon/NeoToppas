using MaterialDesignThemes.Wpf.Internal;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;

namespace Template.WPF.ViewModels
{
    public class GenpinLabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactiveCommand DeleteSearchTextBtn { get; }

        public GenpinLabelViewModel(TransitionService contentNavigation)
        {
            SearchText.Value = "";

            DeleteSearchTextBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                SearchText.Value = "";
                return;
            });
        }
    }
}
