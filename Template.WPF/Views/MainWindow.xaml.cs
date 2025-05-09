﻿using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows;
using Template.WPF.ViewModels;
using System.Diagnostics;

namespace Template.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel; // ViewModelを保持
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel; // ViewModelをフィールドに保存
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // 選択されたアイテムを取得
            if (e.NewValue is TreeViewItem selectedItem)
            {
                string selectedname = selectedItem.Name;
                //MessageBox.Show($"選択された項目: {selectedname}", "項目選択", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.TreeSelectedChange(selectedname);
            }
        }
        private void OpenNewWindow_PartsSearch(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "PartsSearch");
        }

        private void OpenNewWindow_License(object sender, RoutedEventArgs e)
        { 
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname,"License");
        }

        private void OpenNewWindow_PartsLabelPrint(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "PartsLabelPrint");
        }

        private void OpenNewWindow_Dashboard(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "Dashboard");
        }
        private void OpenNewWindow_GenpinLabel(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "GenpinLabel");
        }
        private void OpenNewWindow_KibanLabel(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "KibanLabel");
        }
        private void OpenNewWindow_TanabanPrint(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "TanabanPrint");
        }
        private void OpenNewWindow_TanafudaPrint(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "TanafudaPrint");
        }

        private void OpenNewWindow_EtcLabel(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "EtcLabel");
        }
        private void OpenNewWindow_BomConverter(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "BomConverter");
        }
        private void OpenNewWindow_ShippingList(object sender, RoutedEventArgs e)
        {
            String fullname = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(fullname, "ShippingList");
        }
        //TODO:画面追加対応


    }
}