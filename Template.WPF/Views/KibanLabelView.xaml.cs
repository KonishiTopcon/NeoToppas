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
    public partial class KibanLabelView : UserControl
    {
        KibanLabelViewModel vm0;
        public KibanLabelView(KibanLabelViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            vm0 = viewModel;
        }
        private void MassSheetChange(object sender, RoutedEventArgs e)
        {
            vm0.MassSheetChange();
        }
        private void MassPieceChange(object sender, RoutedEventArgs e)
        {
            vm0.MassPieceChange();
        }
        private void ProtoChange(object sender, RoutedEventArgs e)
        {
            vm0.ProtoChange();
        }
    }
}
