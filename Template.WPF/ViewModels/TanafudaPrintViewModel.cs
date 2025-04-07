using ClosedXML.Excel;
using Microsoft.Extensions.Hosting;
using NeoToppas.Infrastructure.BrotherPrinter;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Template.WPF.ViewModels
{
    public class TanafudaPrintViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
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
        public ReactiveCommand PDFBtn { get; }
        public TanafudaPrintViewModel(TransitionService contentNavigation)
        {
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
            PDFBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates") +
                 @"\" + CommonConst.TANAFUDA_TEMPLATE_FILE;
                string outputExcelPath = ($"C\\Toppas4\\Temporary") + @"\" + "棚札印刷用.xlsx";
                // ファイルが使用中かどうかを確認
                try
                {

                    // 1. Excelを開いてデータを入力
                    using (var workbookx = new XLWorkbook(templatePath))
                    {
                        workbookx.SaveAs(outputExcelPath);
                    }

                    using (var workbooky = new XLWorkbook(outputExcelPath))
                    {
                        var worksheetx = workbooky.Worksheet(1);

                        for (int p = 0; p <= 2; p++)
                        {
                            var worksheetn = worksheetx.CopyTo(p.ToString());
                            if (p == 0 && hinmokutext0.Value.Length >= 0)
                            {
                                worksheetn.Cell("B1").Value = shelf0.Value;
                                worksheetn.Cell("A3").Value = hinmoku0.Value;
                                worksheetn.Cell("C5").Value = hinmokutext0.Value;
                            }
                            if (p == 1 && hinmokutext1.Value.Length >= 0)
                            {
                                worksheetn.Cell("B1").Value = shelf1.Value;
                                worksheetn.Cell("A3").Value = hinmoku1.Value;
                                worksheetn.Cell("C5").Value = hinmokutext1.Value;
                            }
                            if (p == 2 && hinmokutext2.Value.Length >= 0)
                            {
                                worksheetn.Cell("B1").Value = shelf2.Value;
                                worksheetn.Cell("A3").Value = hinmoku2.Value;
                                worksheetn.Cell("C5").Value = hinmokutext2.Value;
                            }
                        }

                        worksheetx.Delete();
                        // 保存
                        workbooky.Save();
                        workbooky.Dispose();
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("エクセルファイルの保存に失敗しました。" + Environment.NewLine +
                        "棚札印刷用.xlsxが開かれている場合は閉じてリトライしてください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try { 
                    string excelAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Office", "root", "Office16", "EXCEL.EXE");

                    // Excelアプリケーションを起動し、ファイルパスを引数として渡す
                    Process.Start(excelAppPath, outputExcelPath);
                    System.Windows.MessageBox.Show("棚札印刷用のエクセルを開きます。" + Environment.NewLine +
                        "印刷範囲を「ブック全体」として印刷してください。");
                }
               catch {
                    System.Windows.MessageBox.Show("エクセルファイルを開けませんでした", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }
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
