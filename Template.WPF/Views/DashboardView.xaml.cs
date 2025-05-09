﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Template.WPF.ViewModels;

namespace Template.WPF.Views
{
    /// <summary>
    /// LicenseView.xaml の相互作用ロジック
    /// </summary>
    public partial class DashboardView : UserControl
    {
        DashboardViewModel DViewModel = null!;
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            DViewModel = viewModel;
        }

        private void OpenUsersManual(object sender, MouseButtonEventArgs e)
        {
            DViewModel.OpenUsersManual();
        }
    }
}
