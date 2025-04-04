using MaterialDesignThemes.Wpf.Internal;
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
using Microsoft.Extensions.Hosting;
using System.Windows;
using Toppas4.Services;
using NeoToppas.Infrastructure.BrotherPrinter;

namespace Template.WPF.ViewModels
{
    public class GenpinLabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemCode { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> Rev { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<String> ItemName { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<DateOnly> ReceiptDate { get; } = new ReactivePropertySlim<DateOnly>();
        public ReactivePropertySlim<String> ShelfNo { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<int> NumberOfPartsReceived { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<float> Cost { get; } = new ReactivePropertySlim<float>();
        public ReactivePropertySlim<int> PrintCnt { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<bool> UpdateReelNum { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<int> SplitSize { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> OrderNo { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<string> ReelNo { get; } = new ReactivePropertySlim<String>();

        public ReactiveCommand DeleteSearchTextBtn { get; }
        public ReactiveCommand SearchBtn { get; }
        public ReactiveCommand PrintBtn { get; }
        public ReactiveCommand CancelBtn { get; }

        public GenpinLabelViewModel(TransitionService contentNavigation)
        {
            ResetForm();
            DeleteSearchTextBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                SearchText.Value = "";
                ResetForm();
                return;
            });
            SearchBtn= new ReactiveCommand().WithSubscribe(async () =>
                 {
                     SearchText.Value = "";
                     ItemCode.Value = "T47001441A";
                     Rev.Value = 0;
                     ItemName.Value = "AD9920ARSZ";
                     ReceiptDate.Value = DateOnly.FromDateTime(DateTime.Now);//今日の日時を取得
                     ShelfNo.Value = "IC-J-06";
                     NumberOfPartsReceived.Value = 340;
                     Cost.Value = 220;
                     PrintCnt.Value = 2;
                     UpdateReelNum.Value = true;
                     SplitSize.Value = 170;
                     OrderNo.Value = 1001000;
                     ReelNo.Value = "1D";
                     return;
                 });
            PrintBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                var doc = new bpac.DocumentClass();
                // ファイルの読み込み
                var templateDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
                if (ItemCode.Value.Length<=1)
                {
                    MessageBox.Show("品目コードが異常です！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (PrintCnt.Value <=0)
                {
                    MessageBox.Show("プリント枚数が異常です！", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (doc.Open(templateDirectory + @"\" + CommonConst.GENPINLABEL_TEMPLATE_FILE))
                {
                    for (int pidx = 1; pidx <= PrintCnt.Value; pidx++)
                    {
                        doc.GetObject("ShelfNo").Text = ShelfNo.Value;
                        doc.GetObject("ReceiptDate").Text = ReceiptDate.Value.ToString("yyyy/MM/dd");
                        doc.GetObject("Quantity").Text = $"{pidx}  /  {PrintCnt.Value}";
                        doc.GetObject("ItemCode").Text = ItemCode.Value;
                        doc.GetObject("ItemName").Text = ItemName.Value;
                        doc.GetObject("RevNo").Text = "Rev." + Rev.Value.ToString().PadLeft(2, '0');
                        doc.GetObject("OrderNo").Text = OrderNo.Value.ToString();
                        doc.GetObject("Cost").Text = string.Format("{0:F2}", Cost.Value);
                        if (UpdateReelNum.Value)
                        {
                            doc.GetObject("ReelNo").Text = ReelNo.Value;
                        }
                        else
                        {
                            doc.GetObject("ReelNo").Text = "";
                        }
                        // ラベルの印刷処理などを実行
                        BrotherPrint.Print_Brother(doc);
                    }
                    doc.Close();
                }
                else
                {
                    MessageBox.Show("ラベルテンプレートを開けませんでした。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            });
            CancelBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                SearchText.Value = "";
                ResetForm();
                return;
            });

        }

        private static void Print_Brother(DocumentClass doc)
        {
            doc.StartPrint("", PrintOptionConstants.bpoDefault);
            doc.PrintOut(1, PrintOptionConstants.bpoDefault);
            doc.EndPrint();
        }

        public void ResetForm()
        {
            SearchText.Value = "";
            ItemCode.Value = "";
            Rev.Value = 0;
            ItemName.Value = "";
            ReceiptDate.Value = new DateOnly(1970, 1, 1);
            ShelfNo.Value = "";
            NumberOfPartsReceived.Value = 0;
            Cost.Value = 0;
            PrintCnt.Value = 1;
            UpdateReelNum.Value = false;
            SplitSize.Value = 0;
            OrderNo.Value = 0;
            ReelNo.Value = "";
        }
    }
}
