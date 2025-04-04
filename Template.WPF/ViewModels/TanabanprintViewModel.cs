using Microsoft.Extensions.Hosting;
using NeoToppas.Infrastructure.BrotherPrinter;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xaml;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;
using Toppas4.Services;

namespace Template.WPF.ViewModels
{
    public class TanabanPrintViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<bool> BarcodeExist { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> hinmoku0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmoku1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmoku2 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext2 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf2 { get; } = new ReactivePropertySlim<string>();


        public ReactiveCommand DeleteHinmokuBtn0 { get; }
        public ReactiveCommand DeleteHinmokuBtn1 { get; }
        public ReactiveCommand DeleteHinmokuBtn2 { get; }
        public ReactiveCommand CancelBtn { get; }
        public ReactiveCommand PrintBtn { get; }
        public TanabanPrintViewModel(TransitionService contentNavigation)
        {
            BarcodeExist.Value = false;
            hinmoku0.Value = "T47001441A";
            hinmoku1.Value = "T47001441B";
            hinmoku2.Value = "T47001441C";
            hinmokutext0.Value = "AD9920ARSX";
            hinmokutext1.Value = "AD9920ARSY";
            hinmokutext2.Value = "AD9920ARSZ";
            shelf0.Value = "IC-J-06";
            shelf1.Value = "IC-J-07";
            shelf2.Value = "IC-J-08";

            DeleteHinmokuBtn0 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku0.Value = "";
                hinmokutext0.Value = "";
                shelf0.Value = "";
                return;
            });
            DeleteHinmokuBtn1 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku1.Value = "";
                hinmokutext1.Value = "";
                shelf1.Value = "";
                return;
            });
            DeleteHinmokuBtn2 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku2.Value = "";
                hinmokutext2.Value = "";
                shelf2.Value = "";
                return;
            });
            CancelBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                ResetForm();
                return;
            });
            PrintBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                int MAX_TANABAN_CNT = 3;
                var doc = new bpac.DocumentClass();
                var templateDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
                if (BarcodeExist.Value == true) //ボーコードなし
                {
                    if (!doc.Open(templateDirectory + @"\" + CommonConst.TANABANLABEL_TEMPLATE_FILE))
                    {
                        System.Windows.MessageBox.Show("ラベルテンプレートを開けませんでした。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else //ボーコードあり
                {
                    if (!doc.Open(templateDirectory + @"\" + CommonConst.TANABANLABELB_TEMPLATE_FILE))
                    {
                        System.Windows.MessageBox.Show("ラベルテンプレートを開けませんでした。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                for (int pidx = 1; pidx <= MAX_TANABAN_CNT; pidx++)
                {
                    if (pidx == 1 && hinmokutext0.Value !="")
                    {
                        doc.GetObject("ShelfNo").Text = shelf0.Value;
                        doc.GetObject("ItemCode").Text = hinmoku0.Value;
                        doc.GetObject("ItemName").Text = hinmokutext0.Value;
                        BrotherPrint.Print_Brother(doc);
                    }
                    else if (pidx == 2 && hinmokutext1.Value != "")
                    {
                        doc.GetObject("ShelfNo").Text = shelf1.Value;
                        doc.GetObject("ItemCode").Text = hinmoku1.Value;
                        doc.GetObject("ItemName").Text = hinmokutext1.Value;
                        BrotherPrint.Print_Brother(doc);
                    }
                    else if (pidx == 3 && hinmokutext2.Value != "")
                    {
                        doc.GetObject("ShelfNo").Text = shelf2.Value;
                        doc.GetObject("ItemCode").Text = hinmoku2.Value;
                        doc.GetObject("ItemName").Text = hinmokutext2.Value;
                        BrotherPrint.Print_Brother(doc);
                    }
                    else
                    {
                        break;
                    }
                }
                doc.Close();

                return;
            });

        }

        public void ResetForm()
        {
            hinmoku0.Value = "";
            hinmoku1.Value = "";
            hinmoku2.Value = "";
            hinmokutext0.Value = "";
            hinmokutext1.Value = "";
            hinmokutext2.Value = "";
            shelf0.Value = "";
            shelf1.Value = "";
            shelf2.Value = "";
        }

        public void Hinmoku0_LostFocus()
        {
            if (hinmoku0.Value != "")
            {
                hinmokutext0.Value = "AD9920ARSX";
                shelf0.Value = "IC-J-06";
                return;
            }
        }
        public void Hinmoku1_LostFocus()
        {
            if (hinmoku1.Value != "")
            {
                hinmokutext1.Value = "AD9920ARSY";
                shelf1.Value = "IC-J-07";
                return;
            }
        }
        public void Hinmoku2_LostFocus()
        {
            if (hinmoku2.Value != "")
            {
                hinmokutext2.Value = "AD9920ARSZ";
                shelf2.Value = "IC-J-08";
                return;
            }
        }
    }
}
