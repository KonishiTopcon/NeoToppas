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
using bpac;
using System.Xml.Linq;
using System.Windows;
using NeoToppas.WPF.Services;

namespace Template.WPF.ViewModels
{
    public class LabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MessageService _message;
        private readonly DialogService _dialog;
        public ReactiveCommand PrintLabelButton { get; }
        public ReactivePropertySlim<string> ShelfNo { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<DateTime> ArrivalDate { get; } = new ReactivePropertySlim<DateTime>();
        public ReactivePropertySlim<string> Quantity { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemCode { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemName { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> RevNo { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<string> OrderNo { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<double> Cost { get; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<string> ReelNo { get; } = new ReactivePropertySlim<string>();


        public LabelViewModel(TransitionService contentNavigation)
        {
            ShelfNo.Value = "F060603";
            ArrivalDate.Value = DateTime.Today;
            Quantity.Value = " 500 / 500";
            ItemCode.Value = "T03002712A";
            ItemName.Value = "F931C107MNC";
            RevNo.Value = 0;
            OrderNo.Value = "xxx";
            Cost.Value = 105.0;
            ReelNo.Value = "1D";


            PrintLabelButton = new ReactiveCommand().WithSubscribe(() =>
            {

                var doc = new bpac.DocumentClass();
                // ファイルの読み込み (例: ラベルテンプレートファイル)
                if (doc.Open(CommonConst.TEMPLATE_DIRECTORY + CommonConst.TEMPLATE_FILE))
                {
                    doc.GetObject("ShelfNo").Text = ShelfNo.Value;
                    doc.GetObject("ArrivalDate").Text = ArrivalDate.Value.ToString("yyyy/MM/dd");
                    doc.GetObject("Quantity").Text = Quantity.Value;
                    doc.GetObject("ItemCode").Text = ItemCode.Value;
                    doc.GetObject("ItemName").Text = ItemName.Value;
                    doc.GetObject("RevNo").Text = "Rev." + RevNo.Value.ToString().PadLeft(2, '0');
                    doc.GetObject("OrderNo").Text = OrderNo.Value;
                    doc.GetObject("Cost").Text = string.Format("{0:F2}", Cost.Value);
                    doc.GetObject("ReelNo").Text = ReelNo.Value;
                    // ラベルの印刷処理などを実行
                    doc.StartPrint("", PrintOptionConstants.bpoDefault);
                    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                    doc.EndPrint();
                    doc.Close();
                }
                else
                {
                    MessageBox.Show("ラベルテンプレートを開けませんでした。","エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }
}
