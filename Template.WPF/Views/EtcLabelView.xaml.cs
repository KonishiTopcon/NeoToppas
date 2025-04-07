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
    public partial class EtcLabelView : UserControl
    {
        EtcLabelViewModel vm0;
        public EtcLabelView(EtcLabelViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            vm0 = viewModel;
        }
        private void RadioVersionChange(object sender, RoutedEventArgs e)
        {
            vm0.RadioVersionChange();
        }
        private void RadioSequenceChange(object sender, RoutedEventArgs e)
        {
            vm0.RadioSequenceChange();
        }
        private void RadioBT53QChange(object sender, RoutedEventArgs e)
        {
            vm0.RadioBT53QChange();
        }

        private void Radio3DOCTChange(object sender, RoutedEventArgs e)
        {
            vm0.Radio3DOCTChange();
        }
    }
}
