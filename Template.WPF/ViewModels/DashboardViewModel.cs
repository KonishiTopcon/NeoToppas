using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        /// <summary>
        /// 操作手順書ボタンをクリックしたとき、操作手順書を開く
        /// </summary>
        public void OpenUsersManual()
        {
            try
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string manualsDir = Path.Combine(exePath, "Manuals");

                if (!Directory.Exists(manualsDir))
                {
                    MessageBox.Show("Manualsフォルダが見つかりません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var pdfFile = Directory.EnumerateFiles(manualsDir, "操作手順書*.pdf").FirstOrDefault();

                if (pdfFile != null)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = pdfFile,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("操作手順書のPDFが見つかりません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "例外", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
