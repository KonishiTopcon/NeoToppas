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
    /// HomeView.xaml の相互作用ロジック
    /// </summary>
    public partial class LabelView : UserControl
    {
        public LabelView(LabelViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
