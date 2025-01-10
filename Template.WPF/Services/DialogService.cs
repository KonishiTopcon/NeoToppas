using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Template.WPF.ViewModels;

namespace Template.WPF.Services
{
    public class DialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // モーダル表示
        public bool ShowModalWindow<TView>(object parameter = null) where TView : FrameworkElement
        {
            return ShowWindow<TView>(parameter, isModal: true);
        }

        // モードレス表示
        public void ShowModelessWindow<TView>(object parameter = null) where TView : FrameworkElement
        {
            ShowWindow<TView>(parameter, isModal: false);
        }

        // 共通のウィンドウ表示メソッド
        private bool ShowWindow<TView>(object parameter, bool isModal) where TView : FrameworkElement
        {
            // TViewのインスタンスをDIコンテナから取得
            var view = _serviceProvider.GetRequiredService<TView>();

            if (view == null)
            {
                throw new InvalidOperationException($"The requested view of type {typeof(TView).Name} could not be found. Please ensure it is properly registered in the service container.");
            }

            // CustomDialogWindowを作成し、取得したViewをそのコンテンツとして設定
            var dialogWindow = new MetroWindow
            {
                Title = typeof(TView).Name,
                Content = view,
                SizeToContent = SizeToContent.WidthAndHeight,
                MaxWidth = 800,
                MaxHeight = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = Application.Current.MainWindow,
                ShowMaxRestoreButton = false,
                ShowMinButton = false,
                ResizeMode = ResizeMode.NoResize
            };

            // ViewModelがIRequestCloseを実装しているかを確認
            if (view.DataContext is IDialogAware requestClose)
            {
                // RequestCloseイベントを購読し、発火されたらウィンドウを閉じる
                requestClose.RequestClose += (s, e) =>
                {
                    dialogWindow.DialogResult = e;
                    dialogWindow.Close();
                };
            }

            // パラメータが存在する場合、ViewまたはそのDataContextが INavigationAware を実装しているか確認
            if (parameter != null)
            {
                if (view is IDialogAware dialogAwareView)
                {
                    dialogAwareView.OnDialogOpened(parameter);
                }
                else if (view.DataContext is IDialogAware dialogAwareViewModel)
                {
                    dialogAwareViewModel.OnDialogOpened(parameter);
                }
            }

            // ダイアログをモーダルまたはモードレスで表示
            if (isModal)
            {
                return dialogWindow.ShowDialog() ?? false;
            }
            else
            {
                dialogWindow.Show();
                return true; // モードレスの場合は常にtrueを返す（エラーハンドリングは別途考慮）
            }
        }
    }

}
