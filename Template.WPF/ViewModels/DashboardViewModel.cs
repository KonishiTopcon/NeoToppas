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
using Toppas4.Services;

namespace Template.WPF.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<string> AppVersion { get; } = new ReactivePropertySlim<string>();

        public DashboardViewModel(TransitionService contentNavigation)
        {
            if (CommonConst.AppVersion is not null)
            {
                AppVersion.Value = "Ver " + CommonConst.AppVersion;
            }
        }
    }
}
