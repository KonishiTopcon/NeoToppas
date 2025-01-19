using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows;
using Template.WPF.ViewModels;

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
    }
}