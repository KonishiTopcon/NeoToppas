using System;
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
    public partial class TanabanPrintView : UserControl
    {
        TanabanPrintViewModel viewModel1;
        public TanabanPrintView(TanabanPrintViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel1 = viewModel;
        }

        private void Hinmoku0_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel1.Hinmoku0_LostFocus();
        }
        private void Hinmoku1_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel1.Hinmoku1_LostFocus();
        }
        private void Hinmoku2_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel1.Hinmoku2_LostFocus();
        }
    }
}
